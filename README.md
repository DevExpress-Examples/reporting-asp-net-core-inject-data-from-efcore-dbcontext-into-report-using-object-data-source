# Reporting with Entity Framework Core and Data Injection in ASP.NET Core Application

This project demonstrates how to create and view reports with data obtained from Dependency Injection container objects using Entity Framework Core for data access. ASP.NET Core application with Entity Framework provides data to a report as DbContext object that operates in the scope of an HTTP request whose lifetime is different from the report's lifetime. A report is created in the HTTP request context and starts a background thread to get data and create a document. A report needs data after the initial HTTP request is completed. This means a report cannot use the default DbContext instance that the Entity Framework creates in the scope of HTTP request.

This approach has the following requirements:

- The application needs a repository that provides data to a report.

- The repository's lifetime exceeds the lifetime of the HTTP request that creates the repository.

- A repository requests the ScopedDbContextProvider instance to create DbContext on demand.

- The HTTP request contains information used to filter data. For example, when you use the user ID to restrict access to reports. A repository, instantiated within the HTTP request's scope, stores the user ID so it is available in the filter criteria. 

## Implementation Details

### Data Repository

The application uses the following repositories:
* [MyEnrollmentsReportRepository](CS/xrefcoredemo/Services/MyEnrollmentsReportRepository.cs)
* [CourseListReportRepository](CS/xrefcoredemo/Services/CourseListReportRepository.cs)

These regular POCO repositories supply source data for the Object Data Source bound to a report.

The repository gets the [ScopedDbContextProvider](CS/xrefcoredemo/Services/ScopedDbContextProvider.cs) as a dependency that creates a separate scope in the context of the current request. A scope created with the ScopedDbContextProvider can access data from the background thread outside the HTTP request.


### Object Data Source

Use the Report Wizard to create the Object Data Source with the "schema only" option and bind it to the report.

### Report Resolver

The [Report Resolver](CS/xrefcoredemo/Services/WebDocumentViewerReportResolver.cs) performs the following tasks:

- instantiate a report
- process object data sources in a report with a dependency injector object.

The dependency injector requests a data repository as a service from a service provider for the HTTP request and injects the repository into the Object Data Source.

### Object Data Source Injector

The [ObjectDataSourceInjector](CS/xrefcoredemo/Services/ObjectDataSourceInjector.cs) is a dependency injector that assigns a data source to a report.

### Document Preview in Report Designer

The [CustomPreviewReportCustomizationService](CS/xrefcoredemo/Services/CustomPreviewReportCustomizationService.cs) assigns a data source to a report before the Report Designer generates a document for preview.
 
