using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Reflection;
using System.Collections.Generic;

namespace AzureProvisioning.Code
{
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string DisplayName = req.Query["DisplayName"];
            string MailNickname = req.Query["MailNickname"];
            string UserPrincipalName = req.Query["UserPrincipalName"];
            string InitialPassword = req.Query["InitialPassword"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            DisplayName = DisplayName ?? data?.DisplayName;
            MailNickname = MailNickname ?? data?.MailNickname;
            UserPrincipalName = UserPrincipalName ?? data?.UserPrincipalName;
            InitialPassword = InitialPassword ?? data?.InitialPassword;

            string responseMessage;
            if (DisplayName.IsNullOrEmpty() || MailNickname.IsNullOrEmpty() || UserPrincipalName.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            if (InitialPassword.IsNullOrEmpty())
            {
                Random rd = new Random();
                int rand_num = rd.Next(10000, 99999);
                InitialPassword = MailNickname + rand_num.ToString();
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            var user = new User
            {
                AccountEnabled = true,
                DisplayName = DisplayName,
                MailNickname = MailNickname,
                UserPrincipalName = UserPrincipalName,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = InitialPassword
                }
            };


            await graphClient.Users
                .Request()
                .AddAsync(user);

            if (InitialPassword.IsNullOrEmpty()) {
                responseMessage = "User created successfully. Generated password is: " + InitialPassword;
            }
            else { 
                responseMessage = "User created successfully.";
            }

            return new OkObjectResult(responseMessage);
        }
    }



    public static class UpdateUserProperty
    {
        [FunctionName("UpdateUserProperty")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string Identity = req.Query["Identity"];
            string Property = req.Query["Property"];
            string Value = req.Query["Value"];


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            Property = Property ?? data?.Property;
            Value = Value ?? data?.Value;
            Identity = Identity ?? data?.Identity;

            string responseMessage;
            if (Property.IsNullOrEmpty() || Value.IsNullOrEmpty() || Identity.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            User user = new User();

            user.GetType().GetProperty(Property).SetValue(user, Value);

            await graphClient.Users[Identity]
                .Request()
                .UpdateAsync(user);

            responseMessage = "User modified successfully.";

            return new OkObjectResult(responseMessage);
        }
    }


    public static class DeleteUser
    {
        [FunctionName("DeleteUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string UserPrincipalName = req.Query["UserPrincipalName"];


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            UserPrincipalName = UserPrincipalName ?? data?.UserPrincipalName;


            string responseMessage;
            if (UserPrincipalName.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            await graphClient.Users[UserPrincipalName]
                .Request()
                .DeleteAsync();

            responseMessage = "User deleted successfully.";


            return new OkObjectResult(responseMessage);
        }
    }

    public static class AddUserToGroup
    {
        [FunctionName("AddUserToGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string UserPrincipalName = req.Query["UserPrincipalName"];
            string GroupId = req.Query["GroupId"];


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            UserPrincipalName = UserPrincipalName ?? data?.UserPrincipalName;
            GroupId = GroupId ?? data?.GroupId;

            string responseMessage;
            if (UserPrincipalName.IsNullOrEmpty() || GroupId.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            User userToAdd = await graphClient.Users[UserPrincipalName].Request().GetAsync();
            await graphClient.Groups[GroupId].Members.References.Request().AddAsync(userToAdd);
            

            responseMessage = "User added to the group successfully.";


            return new OkObjectResult(responseMessage);
        }
    }


    public static class RemoveUserFromGroup
    {
        [FunctionName("RemoveUserFromGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string UserPrincipalName = req.Query["UserPrincipalName"];
            string GroupId = req.Query["GroupId"];


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            UserPrincipalName = UserPrincipalName ?? data?.UserPrincipalName;
            GroupId = GroupId ?? data?.GroupId;

            string responseMessage;
            if (UserPrincipalName.IsNullOrEmpty() || GroupId.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            User userToAdd = await graphClient.Users[UserPrincipalName].Request().GetAsync();
            
            await graphClient.Groups[GroupId].Members[userToAdd.Id].Reference
                .Request()
                .DeleteAsync();


            responseMessage = "User removed from the group successfully.";


            return new OkObjectResult(responseMessage);
        }
    }


    public static class CreateGroup
    {
        [FunctionName("CreateGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string GroupType = req.Query["GroupType"];
            string GroupName = req.Query["GroupName"];
            string DisplayName = req.Query["DisplayName"];
            string MailNickname = req.Query["MailNickname"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            GroupType = GroupType ?? data?.GroupType;
            GroupName = GroupName ?? data?.GroupName;
            DisplayName = DisplayName ?? data?.DisplayName;
            MailNickname = MailNickname ?? data?.MailNickname;

            string responseMessage;
            if (GroupType.IsNullOrEmpty() || GroupName.IsNullOrEmpty() || DisplayName.IsNullOrEmpty() || MailNickname.IsNullOrEmpty())
            {
                responseMessage = "Missing Parameter.";
                return new BadRequestObjectResult(responseMessage);
            }

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", true)
                    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                    .AddEnvironmentVariables()
                    .Build();


            var tenantId = builder.GetValue<string>("_secret:tenantId");
            var clientId = builder.GetValue<string>("_secret:clientId");
            var clientSecret = builder.GetValue<string>("_secret:clientSecret");

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
            Group group;


            switch (GroupType)
            {
                case "Unified": // OK
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        GroupTypes = new List<String>()
                        {
                            "Unified"
                        },
                        MailEnabled = true,
                        MailNickname = MailNickname,
                        SecurityEnabled = false
                    };
                    break;


                case "UnifiedSecurity": //OK
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        GroupTypes = new List<String>()
                        {
                            "Unified"
                        },
                        MailEnabled = true,
                        MailNickname = MailNickname,
                        SecurityEnabled = true
                    };
                    break;

                case "Security": // OK
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        MailEnabled = false,
                        MailNickname = MailNickname,
                        SecurityEnabled = true
                    };
                    break;
/*
                case "MailEnabledSecurity": //NOK
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        MailEnabled = true,
                        MailNickname = MailNickname,
                        SecurityEnabled = true
                    };
                    break;

                case "Distribution": //NOK
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        MailEnabled = true,
                        MailNickname = MailNickname,
                        SecurityEnabled = false
                    };
                    break;

                case "DynamicUnified": //NOK missing membershipRule
                    group = new Group
                    {
                        DisplayName = DisplayName,
                        GroupTypes = new List<String>()
                        {
                            "Unified",
                            "DynamicMembership"
                        },
                        MailEnabled = true,
                        MailNickname = MailNickname,
                        SecurityEnabled = false
                    };
                    break;
*/
                default:
                    responseMessage = "GroupType not supported.";
                    return new BadRequestObjectResult(responseMessage);

            }

            await graphClient.Groups
                .Request()
                .AddAsync(group);

            responseMessage = "Group created successfully.";


            return new OkObjectResult(responseMessage);
        }
    }

}
