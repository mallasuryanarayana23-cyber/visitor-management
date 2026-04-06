import os
import uuid

def generate_csproj():
    base_dir = r"c:\Users\Surya Narayana\Desktop\visitor management"
    project_guid = str(uuid.uuid4()).upper()

    csproj_start = f"""<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProductVersion>
    </ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{{{project_guid}}}</ProjectGuid>
    <ProjectTypeGuids>{{349c5851-65df-11da-9384-00065b846f21}};{{fae04ec0-301f-11d3-bf4b-00c04f79efbc}}</ProjectTypeGuids>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>VMS</RootNamespace>
    <AssemblyName>VMS</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <MvcBuildViews>false</MvcBuildViews>
    <UseIISExpress>true</UseIISExpress>
    <Use64BitIISExpress />
    <IISExpressSSLPort />
    <IISExpressAnonymousAuthentication />
    <IISExpressWindowsAuthentication />
    <IISExpressUseClassicPipelineMode />
    <UseGlobalApplicationHostFile />
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System" />
    <Reference Include="System.Data" />
    <Reference Include="System.Drawing" />
    <Reference Include="System.Web.DynamicData" />
    <Reference Include="System.Web.Entity" />
    <Reference Include="System.Web.ApplicationServices" />
    <Reference Include="System.ComponentModel.DataAnnotations" />
    <Reference Include="System.Core" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Web" />
    <Reference Include="System.Web.Extensions" />
    <Reference Include="System.Web.Abstractions" />
    <Reference Include="System.Web.Routing" />
    <Reference Include="System.Xml" />
    <Reference Include="System.Configuration" />
    <Reference Include="System.Web.Services" />
    <Reference Include="System.EnterpriseServices" />
    <Reference Include="Microsoft.Web.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.Web.Infrastructure.1.0.0.0\\lib\\net40\\Microsoft.Web.Infrastructure.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.Helpers, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.WebPages.3.2.9\\lib\\net45\\System.Web.Helpers.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.Mvc, Version=5.2.9.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.Mvc.5.2.9\\lib\\net45\\System.Web.Mvc.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.Razor.3.2.9\\lib\\net45\\System.Web.Razor.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.WebPages, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.WebPages.3.2.9\\lib\\net45\\System.Web.WebPages.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.WebPages.Deployment, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.WebPages.3.2.9\\lib\\net45\\System.Web.WebPages.Deployment.dll</HintPath>
    </Reference>
    <Reference Include="System.Web.WebPages.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <Private>True</Private>
      <HintPath>.\\packages\\Microsoft.AspNet.WebPages.3.2.9\\lib\\net45\\System.Web.WebPages.Razor.dll</HintPath>
    </Reference>
    <Reference Include="Oracle.ManagedDataAccess, Version=4.122.21.1, Culture=neutral, PublicKeyToken=89b483f429c47342, processorArchitecture=MSIL">
      <HintPath>.\\packages\\Oracle.ManagedDataAccess.21.11.0\\lib\\net462\\Oracle.ManagedDataAccess.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Content Include="Web.config" />
    <Content Include="Global.asax" />
"""

    csproj_end = """
  </ItemGroup>
  <PropertyGroup>
    <VisualStudioVersion Condition="'$(VisualStudioVersion)' == ''">10.0</VisualStudioVersion>
    <VSToolsPath Condition="'$(VSToolsPath)' == ''">$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
  </PropertyGroup>
  <Import Project="$(MSBuildBinPath)\\Microsoft.CSharp.targets" />
  <Import Project="$(VSToolsPath)\\WebApplications\\Microsoft.WebApplication.targets" Condition="'$(VSToolsPath)' != ''" />
  <Import Project="$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v10.0\\WebApplications\\Microsoft.WebApplication.targets" Condition="false" />
  <ProjectExtensions>
    <VisualStudio>
      <FlavorProperties GUID="{349c5851-65df-11da-9384-00065b846f21}">
        <WebProjectProperties>
          <UseIIS>True</UseIIS>
          <AutoAssignPort>True</AutoAssignPort>
          <DevelopmentServerPort>5000</DevelopmentServerPort>
          <DevelopmentServerVPath>/</DevelopmentServerVPath>
          <IISUrl>http://localhost:5000/</IISUrl>
          <NTLMAuthentication>False</NTLMAuthentication>
          <UseCustomServer>False</UseCustomServer>
          <CustomServerUrl>
          </CustomServerUrl>
          <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
        </WebProjectProperties>
      </FlavorProperties>
    </VisualStudio>
  </ProjectExtensions>
</Project>
"""

    compile_items = []
    content_items = []

    for root, dirs, files in os.walk(base_dir):
        if "packages" in root or "bin" in root or "obj" in root or ".gemini" in root:
            continue
        for f in files:
            full_path = os.path.join(root, f)
            rel_path = os.path.relpath(full_path, base_dir)
            if rel_path == "Global.asax" or rel_path == "Web.config":
                continue # Already added manually
            
            if f.endswith('.cs'):
                compile_items.append(f'    <Compile Include="{rel_path}" />')
            elif f.endswith('.cshtml') or f.endswith('.html') or f.endswith('.css') or f.endswith('.js'):
                content_items.append(f'    <Content Include="{rel_path}" />')

    with open(os.path.join(base_dir, "visitor_management.csproj"), "w", encoding="utf-8") as f:
        f.write(csproj_start)
        f.write("\n".join(compile_items) + "\n")
        f.write("  </ItemGroup>\n  <ItemGroup>\n")
        f.write("\n".join(content_items) + "\n")
        f.write(csproj_end)

if __name__ == "__main__":
    generate_csproj()
    print("visitor_management.csproj generated successfully.")
