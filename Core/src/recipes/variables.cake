///////////////////////////////////////////////////////////////////////////////
// ENVIRONMENT VARIABLE NAMES
///////////////////////////////////////////////////////////////////////////////
static string slackTokenVariable = "SLACK_TOKEN";
static string slackChannelVariable = "SLACK_CHANNEL";

static string nuGetApiKeyVariable = "KLONDIKE_API_KEY";
static string nuGetSourceUrlVariable = "KLONDIKE_URL";
static string nuGetSourceCIUrlVariable = "KLONDIKE_CI_URL";
static string nuGetApiKeyCIUrlVariable = "KLONDIKE_CI_API_KEY";
static string nuGetSourcesVariable = "NUGET_SOURCES";
static string nuGetSymbolSourceUrlVariable = "NUGET_PUSH_SOURCE_SYMBOLS";

static string octopusUrlVariable = "OCTOPUS_URL";
static string octopusUrlVariableDefaultValue  = "deploy.mk6.local";
static string octopusApiKeyVariable = "OCTOPUS_API_KEY";
static string octopusApiKeyVariableDefaultValue = "API-XXX";

static string nugetEnableSymbols = "NUGET_ENABLE_SYMBOLS";
static string nugetEnableSymbolsDefaultValue = "NUGET_ENABLE_SYMBOLS";

static string localNugetSourceVariable = "LOCAL_NUGET_SOURCE";
static string localNugetSourceDefaultValue = "c:\\packages";
static string klondikeUrlDefault ="http://packages.mk6.local/odata/api";
static string klondikeCIUrlDefault ="http://packages.mk6.local:8888/odata/api";
static string nugetSourcesDefaultValue = String.Format("{0};{1};{2} ", localNugetSourceDefaultValue, klondikeUrlDefault, klondikeCIUrlDefault);
