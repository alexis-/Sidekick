// <copyright file="PexAssemblyInfo.cs">joe</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "xunit-2.0")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("Sidekick")]
[assembly: PexAssemblyUnderTest("SQLite.Net.Bridge")]
[assembly: PexInstrumentAssembly("System.Runtime.Extensions")]
[assembly: PexInstrumentAssembly("System.Linq.Expressions")]
[assembly: PexInstrumentAssembly("System.Threading")]
[assembly: PexInstrumentAssembly("System.Reflection.Extensions")]
[assembly: PexInstrumentAssembly("System.Reflection")]
[assembly: PexInstrumentAssembly("System.Diagnostics.Debug")]
[assembly: PexInstrumentAssembly("System.Runtime")]
[assembly: PexInstrumentAssembly("System.Threading.Tasks")]
[assembly: PexInstrumentAssembly("System.Collections")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime.Extensions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Linq.Expressions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Threading")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Reflection.Extensions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Reflection")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Diagnostics.Debug")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Threading.Tasks")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Collections")]

