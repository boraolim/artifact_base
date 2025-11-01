namespace Utilities.Core.Shared.Constants;

public static class RegexConstants
{
    public const string REGEX_IS_REPOSITORY_END_WITH = @"^.*Repository$";
    public const string REGEX_IS_INTERFACE_TO_REPOSITORY_START_WITH = @"^I.*Repository$";
    public const string REGEX_IS_SERVICE_END_WITH = @"^.*Service$";
    public const string REGEX_IS_INTERFACE_TO_SERVICE_START_WITH = @"^I.*Service$";
    public const string REGEX_IS_MAPPER_END_WITH = @"^.*Mapper$";
    public const string REGEX_IS_INTERFACE_TO_MAPPER_START_WITH = @"^I.*Mapper$";
    public const string REGEX_IS_VALIDATOR_END_WITH = @"^.*Validator$";
    public const string REGEX_IS_INTERFACE_TO_VALIDATOR_START_WITH = @"^I.*Validator$";
    public const string REGEX_IS_DEPENDENCY_PATTERN = @"^.*Dependencies$";
    public const string REGEX_IS_INTERGACE_TO_DEPENDENCY_START_WITH = @"^I.*Dependencies$";
    public const string RGX_ALPHA_PATTERN = @"^[\w\s]{10,255}$";
    public const string RGX_USERNAME_PATTERN = @"^(?!.*[._]{2})(?!.*[._]$)[a-zA-Z0-9._]{8,15}$";
    public const string RGX_PASSWORD_PATTERN_V1 = @"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\s]).{8,25}$";
    public const string RGX_PASSWORD_PATTERN_V2 = @"^(?!.*(.).*\1)(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#!_?¿¡\-])[A-Za-z\d@#!_?¿¡\-]{8,}$";
    public const string RGX_PIN_PATTERN = @"^\d{4}$|^\d{6}$";
    public const string RGX_LOCATION_PATTERN = @"^([-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)),\s*([-+]?(180(\.0+)?|(1[0-7]\d|\d{1,2})(\.\d+)?))$";
    public const string RGX_UUID_V4_PATTERN = @"^[{(]?[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){1}-4[0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}(-[0-9a-fA-F]{12})[)}]?$";
    public const string RGX_DATE_DASH_YMD_HM_PATTERN = @"[0-9]{4}-(((0[13578]|(10|12))-(0[1-9]|[1-2][0-9]|3[0-1]))|(02-(0[1-9]|[1-2][0-9]))|((0[469]|11)-(0[1-9]|[1-2][0-9]|30)))\s[0-9]{2}[:]{1}[0-9]{2}$";
    public const string RGX_SPACE_CLEAR = @"\s+";
    public const string RGX_MULTIPLE_SPACE_CLEAR = @"^\s+$";
    public const string RGX_JWT_TOKEN = @"^Bearer\s[a-zA-Z0-9\-\._~\+\/]+=*$";
    public const string RGX_EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
}
