---
page_type: sample
languages:
- csharp
products:
- dotnet
description: "Customer key store for RMS Double Key Encryption"
urlFragment: ""
---

# Customer Key Store to enable a second key for use in Azure RMS

<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

The customer key store is used for double key encryption in Azure RMS.  This key is kept under your control and not exposed to Microsoft.  

test 

## Contents

Outline the file contents of the repository. It helps users navigate the codebase, build configuration and any related assets.

| File/folder       | Description                                |
|-------------------|--------------------------------------------|
| `src`             | Sample source code.                        |
| `.gitignore`      | Define what to ignore at commit time.      |
| `CHANGELOG.md`    | List of changes to the sample.             |
| `CONTRIBUTING.md` | Guidelines for contributing to the sample. |
| `README.md`       | This README file.                          |
| `LICENSE`         | The license for the sample.                |

## Prerequisites

- Install .NET Core 2.1 SDK by following the instructions at [dot.net/core](https://dotnet.microsoft.com/download/dotnet-core/2.1)
- Install [Visual Studio Code](https://code.visualstudio.com/)

## Setup

- In Visual Studio Code extensions install C# and nuget package manager

## Running the sample

- Load the project in Visual Studio Code
- Open appsettings.json
- Under 'AzureAd' section replace \<tenantid\> in 'ValidIssuers' with your Azure AD tenant ID
- Under 'TestKeys' section modify the following:
    - Change the 'Name' value
    - Change the 'Id' value to contain a GUID
    - If running on prem then add a group in your Active Directory to 'AuthorizedRoles' that should be able to access this key.
        Roles should be comma delimited.  Ex. "AuthorizedRoles": ["role1","role2"]
    - Or you can add a list of email address to 'AuthorizedEmailAddress' that should have access to the key
        Emails should be comma delimited. Ex. "AuthorizedEmailAddress": ["email1@company.com","email2@company.com"]
    - Remove whichever one you did not use - 'AuthorizedEmailAddress' or 'AuthorizedRoles'.  Both cannot be present
    - Modify the value of 'PublicPem' to be a valid public key in PEM format, on a single line
    - Modify the value of 'PrivatePem' to be a valid private key in PEM format, on a single line
- Go to Debug -> Start Debugging

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

[Coding Guidelines](https://blogs.msdn.microsoft.com/brada/2005/01/26/internal-coding-guidelines/)
[Capitalization Conventions](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions)
