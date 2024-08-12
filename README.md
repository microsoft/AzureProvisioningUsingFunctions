# Azure Active Directory Provisioning with Azure Functions


[![.NET](https://github.com/microsoft/AzureProvisioningUsingFunctions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/microsoft/AzureProvisioningUsingFunctions/actions/workflows/dotnet.yml)

The code provided by this project can be used to create Azure Functions to provision quickly, with a simple GET or POST call users and groups to Azure AD and execute management activities on the resources.


The following Functions are provided:

- **CreateUser**: Create a new Azure AD user with a password and ForceChangePasswordNextSignIn property to True. The function takes the following parameter:
	- **DisplayName (Mandatory)** - This will be the DisplayName assigned to the user.
	- **MailNickname (Mandatory)** - This will be the MailNickName assigned to the user.
	- **UserPrincipalName (Mandatory)** - This will be the UserPrincipalName assigned to the user. The assigned domain MUST be one of the registered domain in the tenant.
	- **InitialPassword (Optional)** - This will be the password assigned to the user for first login. If not provided, a random password will be assigned and returned by the function.

- **UpdateUserProperty**: Modify an Azure AD user attribute
	- **Identity (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to modify.
	- **Property (Mandatory)** - This will be the Property (Attribute) of the user that will be modified.
	- **Value (Mandatory)** - This will be the Value of Property that will be modified.

- **DeleteUser**: Delete an Azure AD user identified by the UserPrincipalName. The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to delete.

- **CreateGroup**: Create an Azure AD group of specified type. The function takes the following parameter:
	- **GroupType (Mandatory)** - The GroupType can be Unified (SecurityEnabled=False,MailEnabled=True),UnifiedSecurity (SecurityEnabled=True,MailEnabled=True) or Security (SecurityEnabled=True,MailEnabled=False).
	- **GroupName (Mandatory)** - The name to assign to the group.
	- **DisplayName (Mandatory)** - The DisplayName to assign to the group.
	- **MailNickname (Mandatory)** - The MailNickname to assign to the group.

- **AddUserToGroup**: Add an Azure AD user identified by the UserPrincipalName to a group. The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to add to the group.
	- **GroupId (Mandatory)** - This will be the GroupId of the group where the user will be added.

- **RemoveUserFromGroup**: Remove an Azure AD user identified by the UserPrincipalName to a group. The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to add to the group.
	- **GroupId (Mandatory)** - This will be the GroupId of the group where the user will be added.

- **UpdateGroupProperty**: Modify an Azure AD group attribute
	- **GroupId (Mandatory)** - This will be the GroupId of the group you want to modify.
	- **Property (Mandatory)** - This will be the Property (Attribute) of the user that will be modified.
	- **Value (Mandatory)** - This will be the Value of Property that will be modified.

	
- **CreateGuestUser**: Create a guest user in Azure AD 
	- **InvitedUserEmailAddress (Mandatory)** - This will be the email address address of the user you want to add to you Azure AD as Guest user.
	- **SendInvitationMessage (Optional)** - It can be TRUE or FALSE and let you decide if you want to send or not invitation message to guest user. Default is TRUE.


- **DownloadTeamsChat**: Download all user, identified by the UserPrincipalName, Teams chats in JSON format. WARNING: This is a Teams protected API (see https://docs.microsoft.com/en-us/graph/teams-protected-apis). The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to export Teams chats.
	- **DownloadFormat (Optional)** - This will be format of the chat export (future use). Right now only the JSON format is supported.

	
- **GetUserSignInInfoSuccess**: Gets available user's, identified by the UserPrincipalName, succeded sign-in information (interactive only). The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to export sign-in information.
	- **DownloadFormat (Optional)** - This will be format of the export (future use). Right now only the JSON format is supported (default).

	
- **GetUserSignInInfoFailure**: Gets available user's, identified by the UserPrincipalName, failed sign-in information (interactive only). The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to export sign-in information.
	- **DownloadFormat (Optional)** - This will be format of the export (future use). Right now only the JSON format is supported (default).

- **GetUserSignInInfo**: Gets available user's, identified by the UserPrincipalName, sign-in information (interactive only). The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to export sign-in information.
	- **DownloadFormat (Optional)** - This will be format of the export (future use). Right now only the JSON format is supported (default).


- **GetUserEarliestSignInByResource**: Gets earliest available user's, identified by the UserPrincipalName, succeded sign-in information (interactive only). The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to export sign-in information.
	- **DownloadFormat (Optional)** - This will be format of the export (future use). Right now only the JSON format is supported. If not specified, a CSV format is returned in the response.




## Setup & Prerequisites
Using the steps below you will be able to create all prerequisites and resources required by the solution.


### Create App Registration
Registering your application establishes a trust relationship between your app and the Microsoft identity platform. The trust is unidirectional: your app trusts the Microsoft identity platform, and not the other way around.

Follow these steps to create the app registration:

	1. Sign in to the Azure portal.
	2. Search for and select Azure Active Directory.
	3. Under Manage, select App registrations > New registration.
	4. Enter a display Name for your application. Users of your application might see the display name when they use the app, for example during sign-in. 
	5. Note the automatically generated Application (client) ID.
	6. Don't enter anything for Redirect URI.
	7. Select Register to complete the initial app registration.
	8. Select from left menu of the app created Certificates & secrets > Client secrets > New client secret.
	9. Add a description for your client secret.
	10. Select an expiration for the secret or specify a custom lifetime.
	11. Select Add.
	12. Record the secret's value for use in your client application code. This secret value is never displayed again after you leave this page.




### Authorize app access to Azure Active Directory

	1. From the created app, select from left menu of the app created API Authorization.
	2. Add Graph Authorization: User.ReadWrite.All, LicenseAssignment.ReadWrite.All, Group.ReadWrite.All, Group.Create, Chat.Read.All
	3. Provide Admin consent for the authorizations



### Create Azure Function environment
	1. From the Azure portal menu or the Home page, select Create a resource.
	2. In the New page, select Compute > Function App.
	3. On the Basics page, complete the function app settings.
	4. on others pages, complete the function app settings.
	5. Select Review + create to review the app configuration selections.
	6. On the Review + create page, review your settings, and then select Create to provision and deploy the function app.
	7. Select the Notifications icon in the upper-right corner of the portal and watch for the Deployment succeeded message.
	8. Select Go to resource to view your new function app.
	9. Select the Configuration page from the left menu of the create app.
	10. Select New Application setting and add the entries reported in the table below.


|Name                    |Value                                       |
| :--------------------- | :----------------------------------------- |
|_secret:clientId        | Enter the ClientID created previously      |
|_secret:clientSecret    | Enter the clientSecret created previously  |
|_secret:tenantId        | Enter the tenantId                         |

### Deploy Azure Functions
Deploy Azure Functions for example using Visual Studio.


## Usage
Once deployed to Azure Functions you can execute the funcionts with the following example link:

https://yourfunctionsurl.azurewebsites.net/api/DeleteUser?code=Your-Function-Auth-Code&UserPrincipalName=UserPrincipalName-you-want-to-delete


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
