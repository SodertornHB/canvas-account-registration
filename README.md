# Overview / Introduction
This is an application where users can register their EduId accounts. Administrators can then integrate the account in Canvas LMS.

# Getting Started
To start using this code, follow these steps to clone the repository, set up the database, and configure the project locally.

## Clone the Repository
Open a terminal and run the following command to clone the repository:
> git clone https://github.com/SodertornHB/canvas-account-registration.git
cd group-to-section

## Create the Database
This project requires a database to store data. Use the SQL script provided to create the necessary database and tables.
1. Open your database management tool (such as SQL Server Management Studio).
1. Create a new database
1. Locate the SQL create script at ./CanvasAccountRegistration.Web/Migration/Migrations.sql.
1. Run the script to set up the database schema.

## Install Dependencies
Ensure that you have .NET Core installed. Run the following command in the project directory to restore all required packages:
> dotnet restore 

## Configuration
To run this application, you need to configure your `appsettings.json` file with the correct settings for your environment. A template file, `appsettings.json.template`, is provided in the repository to guide you through this process.

### Create a Configuration File
- Copy the template file and rename it to appsettings.json.

- Open appsettings.json and update the fields with your specific settings as follows:

#### 1. Database Connection
- Under ConnectionStrings, replace SERVER_NAME with the name of your database server and DATABASE_NAME with the name of your database.

#### 2. Application Settings
**Name**: Enter a name for your application.

**KeepLogsInDays**: Define the number of days to retain logs.

**KeysFolder**: Specify the folder path where any necessary keys are stored.

#### 3. Canvas Settings
**ApiHost**: Enter the endpoint URL for your Canvas API.

**Host**: Enter Canvas host.

# Running the Application
After configuration, you can start the application using the following command:
> dotnet run 

# Organizational-Specific Files and Configuration
To maintain organization-specific files and configurations separate from the main codebase, this project supports an organizational-specific folder in the root of the solution. Files in this folder will be copied to the corresponding folders in the web project during the build process, allowing for custom settings without modifying the core code. This approach ensures that organization-specific files and configurations are not overwritten when pulling new code from GitHub. It also keeps your customizations separate, making updates and maintenance easier.

## Setting Up the Organizational-Specific Folder
1. In the root of the solution, create a folder named `organizational-specific`.
1. Inside this folder, add any files or configurations specific to your organization. The folder structure should mirror the structure of the web project. The following file types are automatically copied: `.css`, `.js`, `.cs`, `.json`, `.csproj`, `.resx`, `.xml`

#### Example: Custom appsettings.development.json
If you want to use a custom `appsettings.development.json` file, place it directly in the organizational-specific folder. During the build process, it will be copied to the appropriate location in the web project.

### Important Note
Any files in the web project that have the same name and path as files in the organizational-specific folder will be overwritten by the files from organizational-specific during the build process. Be cautious when adding files to avoid unintentional overwrites.

## Configuration in the Project File
The copying of files is configured in the .csproj file. Below is the configuration that enables this copying process:
```xml
  <Target Name="CopyOrgSpecificFiles" BeforeTargets="Build">
    <ItemGroup>
      <OrgSpecificFiles Include="..\organizational-specific\**\*.css" />
      <OrgSpecificFiles Include="..\organizational-specific\**\*.js" />
      <OrgSpecificFiles Include="..\organizational-specific\**\*.cs" />
      <OrgSpecificFiles Include="..\organizational-specific\**\*.xml" />
      <OrgSpecificFiles Include="..\organizational-specific\*.json" />
      <OrgSpecificFiles Include="..\organizational-specific\*.csproj" />
    </ItemGroup>

    <Message Text="Copying @(OrgSpecificFiles) to $(ProjectDir)%(OrgSpecificFiles.RecursiveDir)%(Filename)%(Extension)" Importance="high" />

    <Copy SourceFiles="@(OrgSpecificFiles)" DestinationFiles="@(OrgSpecificFiles->'$(ProjectDir)%(RecursiveDir)%(Filename)%(Extension)')" OverwriteReadOnlyFiles="true" />
  </Target>
```

# Contributing to the Project
We welcome contributions to this open-source project! By contributing, you help improve the functionality, usability, and reliability of this solution for other users. Follow these steps to get started with your contribution.

## Contribution Guidelines
### 1. Fork the Repository

Start by forking the repository to your own GitHub account. This creates a copy of the project where you can make your changes.
### 2. Clone Your Forked Repository

Clone your forked repository to your local machine using the command:
```bash
git clone https://github.com/YOUR_USERNAME/canvas-account-registration.git  
```
Replace YOUR_USERNAME with your GitHub username.
### 3. Create a New Branch

To keep your changes organized, create a new branch for each feature or bug fix you want to work on:
```bash
git checkout -b feature/your-feature-name  
```
Use a descriptive name for your branch, like feature/add-payment-option or bugfix/fix-currency-bug.
### 4. Make Your Changes

Implement your changes in the codebase. Ensure your code follows the project's coding standards and is properly documented.
### 5. Test Your Changes

Before submitting your contribution, test your changes thoroughly. If applicable, add or update unit tests to maintain code quality.
### 6. Commit and Push Your Changes

Stage and commit your changes with a descriptive commit message:
```bash
git add .  
git commit -m Add description of your changes  
```
Push your changes to your forked repository:

```bash
git push origin feature/your-feature-name  
```
### 7. Submit a Pull Request

Go to the original repository on GitHub and submit a pull request from your forked repository. Include a clear description of your changes, why they are necessary, and any relevant context.
### 8. Respond to Feedback

Project maintainers may review your pull request and provide feedback or request changes. Please be prepared to make revisions if necessary.