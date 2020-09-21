---
page_type: sample
languages:
- csharp
products:
- dotnet
description: "Key store for Double Key Encryption"
urlFragment: ""
---

# Customer Key Store to enable a second key for use in Azure RMS

<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

The key store is used for double key encryption for Microsoft 365. You keep the key under your control. The key is never exposed to Microsoft.

## Warning

IMPORTANT NOTICE: This project includes code for encryption libraries. You are responsible for complying with all applicable international and national laws that apply to this software, including the U.S. Export Administration Regulations, as well as end-user, end use and destination restrictions by U.S. and other governments.

## Contents

| File/folder         | Description                                                                             |
|---------------------|-----------------------------------------------------------------------------------------|
| `src`               | Contains the DKE service source code.                                                   |
| `.gitignore`        | Identifies what to ignore at commit time.                                               |
| `CODE_OF_CONDUCT.md`| Outlines expectations for participation in Microsoft-managed open source communities.   |
| `README.md`         | This README file.                                                                       |
| `LICENSE`           | The license for the DKE service software.                                               |
| `SECURITY.md`       | Describes how to contact Microsoft to report a security vulnerability.                  |

## Prerequisites

- Install .NET Core 2.1 SDK by following the instructions at [dot.net/core](https://dotnet.microsoft.com/download/dotnet-core/2.1)
- Install [Visual Studio Code](https://code.visualstudio.com/)

## Setup

- In Visual Studio Code extensions install C# and NuGet package manager

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
[Capitalization Conventions](https://docs.microsoft.com/dotnet/standard/design-guidelines/capitalization-conventions)
