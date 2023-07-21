# MagicVilla
This project is my actual full implementation of DotNetMastery's Magic Villa API and Magic Villa MVC.

This dual solution consisting of two projects (plus a shared Utilities project containing only static details) consists of an API Project where we expose or serve API endpoints for a lodging service, and an MVC project that consumes the API.

## MagicVillaAPI Project - Serving or Exposing the API

The API consists of three Controllers. One exposing endpoints for the Villas (Villa API Controller), one for exposing Villa Numbers -or specific lodges-, which correspond to a model that has a one-to-many relationship with the Villas, that is one Villa can have multiple Villa Numbers or lodges, and the Users Controller which exposes registration and login endpoints to the application.

### Application Features
The API has multiple features including versioning, caching, filtering, pagination, authentication and logging. The project demonstrates native custom logging capabilities as well as a basic SeriLog implementation. Similarly the login and registration - and authentication process is custom. Alternatively, the project implements .NET Identity Scaffolding for a complete solution to the question of Authentication and Authorization. The application works with EF Core and has three basic models for Users, Villas and Villa Numbers.

### Models and DTOs
The application defines several models including Villa, VillaNumber, and User models as well as an APIResponse model, and defines several DTOs which map to the models manually at times, and at times using Automapper (it's configuration file is defined in the root project and is named MappingConfig.cs). The DTOs include VillaDTO, which is used by the Get and Delete endpoints, and VillaCreateDTO - used by the Post endpoint - and VillaUpdateDTO used by the Put and Patch endpoints. The DTOs corresponding to VillaNumber (VillaNumberDTO, VillaNumberCreateDTO, and VillaNumberUpdateDTO) do likewise in the VillaNumber Controller. Additionally, there are RegistrationRequest and LoginRequest DTOs, and LoginResponseDTO (Registration returns a 200 response and requires no DTO).

### Repository Pattern
The application also features the Repository Pattern and includes an IRepository interface and Repository class for the core operations, and several other interfaces with their corresponding repositories. IVillaRepository and VillaRepository and IVillaNumberRepository and VillaNumberRepository define their own Update methods since these tend to be custom and vary by model. In addition there's an IUserRepository and UserRepository which define methods for Registration and Login to the API.

### Packages
API specific - Controller level - dependency injection is used throughout as are custom data annotations for the Controller endpoints. Apart from the SeriLog, Automapper and EF Core and other .NET specific packages. The application also uses Json manipulation related packages for the Patch request and more generally for dealing with Json, as well as a package used for Authentication Tokens(Jwt Bearer). 

## MagicVillaWeb Project - Consuming the API

The MagicVillaWeb Project is an MVC project designed to Consumme the MagicVillaAPI. It has several controllers including Villa and VillaNumber Controllers, which have standard CRUD MVC methods to consume the information provided by the exposed API. These includes methods to Get, Create, Update and Delete Villas and Villa Numbers. Since this is an MVC project we have HttpGet and HttpPost versions of some of the methods which are connected to the relevant views such as to display the relevant forms to perform the CRUD operations. Since the MVC Project consumes the API, there is no EF Core or Database associated with this Project.

### Models, DTOs and ViewModels
The Models include the APIRequest model and APIResponse model. The DTOs must coincide with the DTOs defined in the API. Thus, we have VillaDTO, VillaNumberDTO, VillaCreateDTO, VillaNumberCreateDTO, VillaUpdateDTO, and VillaNumberUpdateDTO defined. In addition we have View models which serve to encapsulate data to display in the Views. In this case, the view models are related to VillaNumber, since we need to display a dropdown with the Villas, which we gather programatically and project into the IEnumerable Property "Villa List" in each of the VM. The VM are VillaNumberCreateVM, VillaNumberUpdateVM, and VillaNumberDeleteVM.

### Services
Instead of repositories, we have services. We have a BaseService class (and its corresponding interface), which sends the Request and Processes the response. And we have subclasses -also services - like VillaService and VillaNumberService which configure the APIRequest to be sent to the base service depending on whether its a GetAll, Get, Create, Update or Delete request.

### Views
There is much in the way of Views. There are partial shared views such as Notification, ValidationScriptsPartial (to perform validations on the forms), and Layout. And then we have our main views, for the three main controllers. We have an Index view for the Home Controller where the villas and their details are displayed. But then we have Views for Villa and VillaNumber, which both have their corresponding Index view to view the Villas and the Lodges(Villa Numbers) associated to the Villas, as well as views for Create, Update and Delete.

## In Sum
MagicVillaAPI and MagicVillaWeb are associated projects. MagicVillaWeb depends on MagicVillaAPI to consume and display the data it needs. MagicVillaAPI can be operated independently via the Swagger UI when you run the project, or by using an external tool such as Postman. They are both excellent projects that teach the fundamentals of working with APIs. Although at times the code gets unwieldy - particularly with multiple try catch blocks in the VillaAPIController of the API Project (and then some in the BaseService of the MVC project), the Solution overall exhibits great architectural form and separation of concerns, modularity and loose coupling.

