<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T883610)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# Reporting for ASP.NET Core - Inject Data from the Entity Framework Core DbContext into a Report Using the Object Data Source

An ASP.NET Core application with Entity Framework supplies data to a report as a DbContext object. This object operates in the scope of an HTTP request whose lifetime is different from report lifetime. A report is created in the HTTP request context and starts a background thread to get data and create a document. A report needs data after the initial HTTP request is completed. This means a report cannot use the default DbContext instance that the Entity Framework creates in the scope of the HTTP request.

This example demonstrates the approach that addresses the issues described above. The approach has the following requirements:

- The application needs a repository that supplies data to a report.

- Repository lifetime exceeds the lifetime of the HTTP request that creates the repository.

- A repository requests the ScopedDbContextProvider instance to create DbContext on demand.

- The HTTP request contains information used to filter data. For example, when you use the user ID to restrict access to reports, a repository instantiated within the HTTP request's scope stores the user ID, so it is available in the filter criteria. 

- The repository reads and saves values available in the HTTP request context. The values are stored for later use, so the repository saves the current user ID instead of the context-dependent IUserService object.

- The repository reads and saves the current user ID in its constructor. The constructor is invoked in the context of the HTTP request and has access to context-dependent data.

> **NOTE**: Before running this example, specify the **DefaultConnection** field in the project's _appsettings.json_ file.

## Implementation Details

### Data Repository

The application uses the *MyEnrollmentsReportRepository* repository implemented in the following file: [MyEnrollmentsReportRepository](CS/xrefcoredemo/Services/MyEnrollmentsReportRepository.cs).

The *MyEnrollmentsReportRepository* repository is a regular POCO repository that supplies source data for the Object Data Source bound to a report. The repository gets the [ScopedDbContextProvider](CS/xrefcoredemo/Services/ScopedDbContextProvider.cs) as a dependency that creates a separate scope in the context of the current request. A scope created with the ScopedDbContextProvider can access data from the background thread outside the HTTP request.

### Object Data Source

Use the **Report Wizard** to create the [Object Data Source](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.ObjectBinding.ObjectDataSource) with the "schema only" option and bind the report to this data source.

### Report Resolver

The [Report Resolver](CS/xrefcoredemo/Services/WebDocumentViewerReportResolver.cs) performs the following tasks:

- Instantiates a report.
- Processes object data sources in a report with a dependency injector object.

The dependency injector requests a data repository as a service from a service provider for the HTTP request and injects the repository into the Object Data Source.

### Object Data Source Injector

The [ObjectDataSourceInjector](CS/xrefcoredemo/Services/ObjectDataSourceInjector.cs) is a dependency injector that assigns a data source to a report.

### Document Preview in Report Designer

The [CustomPreviewReportCustomizationService](CS/xrefcoredemo/Services/CustomPreviewReportCustomizationService.cs) assigns a data source to a report before the Report Designer generates a document for preview.
