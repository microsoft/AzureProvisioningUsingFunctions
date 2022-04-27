# Azure Provisioning Functions

The code provided by this project can be used to create Azure Functions to provision quickly, with a simple POST call users (and soon other resources) to Azure AD.

At the moment the following 3 Functions are provided:

- CreateUser: Create a new Azure AD user with a password and ForceChangePasswordNextSignIn property to True. The function takes the following parameter:
	- **DisplayName (Mandatory)** - This will be the DisplayName assigned to the user.
	- **MailNickname (Mandatory)** - This will be the MailNickName assigned to the user.
	- **UserPrincipalName (Mandatory)** - This will be the UserPrincipalName assigned to the user. The assigned domain MUST be one of the registered domain in the tenant.
	- **InitialPassword (Optional)** - This will be the password assigned to the user for first login. If not provided, a random password will be assigned and returned by the function.



- UpdateUserProperty: Modify an Azure AD user attribute
	- **Identity (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to modify.
	- **Property (Mandatory)** - This will be the Property (Attribute) of the user that will be modified.
	- **Value (Mandatory)** - This will be the Value of Property that will be modified.



- DeleteUser: Delete a Azure AD user identified by the UserPrincipalName. The function takes the following parameter:
	- **UserPrincipalName (Mandatory)** - This will be the Identity (using UserPrincipalName) of the user you want to delete.





## Setup
### Create App Registration








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
