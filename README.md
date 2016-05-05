Sample ASP.Net Web API Application with Swashbuckle
=========

This is a sample ASP.Net API application to demmonstrate how to use Swashbuckle to automatically generate Swagger 2.0 docs from *ApiController* types in the project.

**To Run This Example**

* Download the code to the local machine
* Open SwashApiTest.sln with Visual Studio 2015
* F5 to run the application, the home page will open in a browser
* Click "Generate Swagger Doc" on the top bar to see the generated Swagger document
* Click "Swagger UI" on the top bar to see the Swagger UI that can be used to try out the API calls

## Getting Started ##

If you have an ASP.Net Web API application and want to add Swashbuckle support,

* Add NuGet package Swashbuckle.Core to your project
* Copy and paste folder SwashApiTest/Swagger to your project
* F5 to run your project, the generated doc should be at [root]/swagger/docs/2016-04-27dev. The exposed APIs in the Swagger doc are generated from ApiController type controllers.

###To Customize the Swagger Doc Generation###

With Swashbuckle, you can do a lot customizations to your Swagger doc generation.

* Open Swagger/SwaggerConfig.cs, 
  ** Update the line, 
     //swaggerDoc.host = new Uri("http://swashapitest.azurewebsites.net/").Host;
     This will point your Swagger document to a host other than the default.
  ** Update the line, 
     c.SingleApiVersion("2016-04-27dev", "SwashApiTest").Description("Api test client for Swashbuckle");
     This will set your API name, version and description.
  ** Update the line, 
     defaultId = defaultId.Replace("ViewModel", "View");
     Currently, it is renaming XXXViewModel classes to XXXView, just for demonstration. You can change the code here for your own customization.
* Open Swagger/SwaggerSchemaFilter.cs. There is a lot you can here. In the sample, there is code to filter out classes with namespace not starting with "SwashApiTest" and 
     to strip trailing "Internal" out of enum definitions. 
* Open Swagger/SwaggerDocumentFilter.cs. In the sample code, we are adding two global parameters to all calls, 
     and adding a default reponse code to all responses, amoung others.
* F5 to run your project, the generated doc should be at [root]/swagger/docs/[your new api version]
