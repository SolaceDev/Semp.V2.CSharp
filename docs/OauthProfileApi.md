# Semp.V2.CSharp.Api.OauthProfileApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateOauthProfile**](OauthProfileApi.md#createoauthprofile) | **POST** /oauthProfiles | Create an OAuth Profile object.
[**CreateOauthProfileAccessLevelGroup**](OauthProfileApi.md#createoauthprofileaccesslevelgroup) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Create a Group Access Level object.
[**CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileApi.md#createoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
[**CreateOauthProfileClientAllowedHost**](OauthProfileApi.md#createoauthprofileclientallowedhost) | **POST** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Create an Allowed Host Value object.
[**CreateOauthProfileClientAuthorizationParameter**](OauthProfileApi.md#createoauthprofileclientauthorizationparameter) | **POST** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Create an Authorization Parameter object.
[**CreateOauthProfileClientRequiredClaim**](OauthProfileApi.md#createoauthprofileclientrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
[**CreateOauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileApi.md#createoauthprofiledefaultmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
[**CreateOauthProfileResourceServerRequiredClaim**](OauthProfileApi.md#createoauthprofileresourceserverrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
[**DeleteOauthProfile**](OauthProfileApi.md#deleteoauthprofile) | **DELETE** /oauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
[**DeleteOauthProfileAccessLevelGroup**](OauthProfileApi.md#deleteoauthprofileaccesslevelgroup) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Delete a Group Access Level object.
[**DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileApi.md#deleteoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
[**DeleteOauthProfileClientAllowedHost**](OauthProfileApi.md#deleteoauthprofileclientallowedhost) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Delete an Allowed Host Value object.
[**DeleteOauthProfileClientAuthorizationParameter**](OauthProfileApi.md#deleteoauthprofileclientauthorizationparameter) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Delete an Authorization Parameter object.
[**DeleteOauthProfileClientRequiredClaim**](OauthProfileApi.md#deleteoauthprofileclientrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
[**DeleteOauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileApi.md#deleteoauthprofiledefaultmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
[**DeleteOauthProfileResourceServerRequiredClaim**](OauthProfileApi.md#deleteoauthprofileresourceserverrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
[**GetOauthProfile**](OauthProfileApi.md#getoauthprofile) | **GET** /oauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
[**GetOauthProfileAccessLevelGroup**](OauthProfileApi.md#getoauthprofileaccesslevelgroup) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Get a Group Access Level object.
[**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
[**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions**](OauthProfileApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
[**GetOauthProfileAccessLevelGroups**](OauthProfileApi.md#getoauthprofileaccesslevelgroups) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Get a list of Group Access Level objects.
[**GetOauthProfileClientAllowedHost**](OauthProfileApi.md#getoauthprofileclientallowedhost) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Get an Allowed Host Value object.
[**GetOauthProfileClientAllowedHosts**](OauthProfileApi.md#getoauthprofileclientallowedhosts) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Get a list of Allowed Host Value objects.
[**GetOauthProfileClientAuthorizationParameter**](OauthProfileApi.md#getoauthprofileclientauthorizationparameter) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Get an Authorization Parameter object.
[**GetOauthProfileClientAuthorizationParameters**](OauthProfileApi.md#getoauthprofileclientauthorizationparameters) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Get a list of Authorization Parameter objects.
[**GetOauthProfileClientRequiredClaim**](OauthProfileApi.md#getoauthprofileclientrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
[**GetOauthProfileClientRequiredClaims**](OauthProfileApi.md#getoauthprofileclientrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
[**GetOauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileApi.md#getoauthprofiledefaultmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
[**GetOauthProfileDefaultMsgVpnAccessLevelExceptions**](OauthProfileApi.md#getoauthprofiledefaultmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
[**GetOauthProfileResourceServerRequiredClaim**](OauthProfileApi.md#getoauthprofileresourceserverrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
[**GetOauthProfileResourceServerRequiredClaims**](OauthProfileApi.md#getoauthprofileresourceserverrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
[**GetOauthProfiles**](OauthProfileApi.md#getoauthprofiles) | **GET** /oauthProfiles | Get a list of OAuth Profile objects.
[**ReplaceOauthProfile**](OauthProfileApi.md#replaceoauthprofile) | **PUT** /oauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
[**ReplaceOauthProfileAccessLevelGroup**](OauthProfileApi.md#replaceoauthprofileaccesslevelgroup) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Replace a Group Access Level object.
[**ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileApi.md#replaceoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
[**ReplaceOauthProfileClientAuthorizationParameter**](OauthProfileApi.md#replaceoauthprofileclientauthorizationparameter) | **PUT** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Replace an Authorization Parameter object.
[**ReplaceOauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileApi.md#replaceoauthprofiledefaultmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
[**UpdateOauthProfile**](OauthProfileApi.md#updateoauthprofile) | **PATCH** /oauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
[**UpdateOauthProfileAccessLevelGroup**](OauthProfileApi.md#updateoauthprofileaccesslevelgroup) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Update a Group Access Level object.
[**UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileApi.md#updateoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.
[**UpdateOauthProfileClientAuthorizationParameter**](OauthProfileApi.md#updateoauthprofileclientauthorizationparameter) | **PATCH** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Update an Authorization Parameter object.
[**UpdateOauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileApi.md#updateoauthprofiledefaultmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.

<a name="createoauthprofile"></a>
# **CreateOauthProfile**
> OauthProfileResponse CreateOauthProfile (OauthProfile body, string opaquePassword = null, List<string> select = null)

Create an OAuth Profile object.

Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfile(); // OauthProfile | The OAuth Profile object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an OAuth Profile object.
                OauthProfileResponse result = apiInstance.CreateOauthProfile(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfile**](OauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResponse**](OauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileaccesslevelgroup"></a>
# **CreateOauthProfileAccessLevelGroup**
> OauthProfileAccessLevelGroupResponse CreateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Group Access Level object.

Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileAccessLevelGroupExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroup(); // OauthProfileAccessLevelGroup | The Group Access Level object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Group Access Level object.
                OauthProfileAccessLevelGroupResponse result = apiInstance.CreateOauthProfileAccessLevelGroup(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileAccessLevelGroup: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroup**](OauthProfileAccessLevelGroup.md)| The Group Access Level object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupResponse**](OauthProfileAccessLevelGroupResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileaccesslevelgroupmsgvpnaccesslevelexception"></a>
# **CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**
> OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)

Create a Message VPN Access-Level Exception object.

Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroupMsgVpnAccessLevelException(); // OauthProfileAccessLevelGroupMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Message VPN Access-Level Exception object.
                OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse result = apiInstance.CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException(body, oauthProfileName, groupName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileAccessLevelGroupMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse**](OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileclientallowedhost"></a>
# **CreateOauthProfileClientAllowedHost**
> OauthProfileClientAllowedHostResponse CreateOauthProfileClientAllowedHost (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create an Allowed Host Value object.

Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileClientAllowedHostExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileClientAllowedHost(); // OauthProfileClientAllowedHost | The Allowed Host Value object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an Allowed Host Value object.
                OauthProfileClientAllowedHostResponse result = apiInstance.CreateOauthProfileClientAllowedHost(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileClientAllowedHost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileClientAllowedHost**](OauthProfileClientAllowedHost.md)| The Allowed Host Value object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAllowedHostResponse**](OauthProfileClientAllowedHostResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileclientauthorizationparameter"></a>
# **CreateOauthProfileClientAuthorizationParameter**
> OauthProfileClientAuthorizationParameterResponse CreateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create an Authorization Parameter object.

Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileClientAuthorizationParameterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileClientAuthorizationParameter(); // OauthProfileClientAuthorizationParameter | The Authorization Parameter object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an Authorization Parameter object.
                OauthProfileClientAuthorizationParameterResponse result = apiInstance.CreateOauthProfileClientAuthorizationParameter(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileClientAuthorizationParameter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileClientAuthorizationParameter**](OauthProfileClientAuthorizationParameter.md)| The Authorization Parameter object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAuthorizationParameterResponse**](OauthProfileClientAuthorizationParameterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileclientrequiredclaim"></a>
# **CreateOauthProfileClientRequiredClaim**
> OauthProfileClientRequiredClaimResponse CreateOauthProfileClientRequiredClaim (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Required Claim object.

Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileClientRequiredClaim(); // OauthProfileClientRequiredClaim | The Required Claim object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Required Claim object.
                OauthProfileClientRequiredClaimResponse result = apiInstance.CreateOauthProfileClientRequiredClaim(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileClientRequiredClaim**](OauthProfileClientRequiredClaim.md)| The Required Claim object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientRequiredClaimResponse**](OauthProfileClientRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofiledefaultmsgvpnaccesslevelexception"></a>
# **CreateOauthProfileDefaultMsgVpnAccessLevelException**
> OauthProfileDefaultMsgVpnAccessLevelExceptionResponse CreateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Message VPN Access-Level Exception object.

Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileDefaultMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileDefaultMsgVpnAccessLevelException(); // OauthProfileDefaultMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Message VPN Access-Level Exception object.
                OauthProfileDefaultMsgVpnAccessLevelExceptionResponse result = apiInstance.CreateOauthProfileDefaultMsgVpnAccessLevelException(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileDefaultMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileDefaultMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileDefaultMsgVpnAccessLevelExceptionResponse**](OauthProfileDefaultMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createoauthprofileresourceserverrequiredclaim"></a>
# **CreateOauthProfileResourceServerRequiredClaim**
> OauthProfileResourceServerRequiredClaimResponse CreateOauthProfileResourceServerRequiredClaim (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Required Claim object.

Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileResourceServerRequiredClaim(); // OauthProfileResourceServerRequiredClaim | The Required Claim object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Required Claim object.
                OauthProfileResourceServerRequiredClaimResponse result = apiInstance.CreateOauthProfileResourceServerRequiredClaim(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.CreateOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileResourceServerRequiredClaim**](OauthProfileResourceServerRequiredClaim.md)| The Required Claim object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResourceServerRequiredClaimResponse**](OauthProfileResourceServerRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofile"></a>
# **DeleteOauthProfile**
> SempMetaOnlyResponse DeleteOauthProfile (string oauthProfileName)

Delete an OAuth Profile object.

Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.

            try
            {
                // Delete an OAuth Profile object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfile(oauthProfileName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileaccesslevelgroup"></a>
# **DeleteOauthProfileAccessLevelGroup**
> SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroup (string oauthProfileName, string groupName)

Delete a Group Access Level object.

Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileAccessLevelGroupExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.

            try
            {
                // Delete a Group Access Level object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileAccessLevelGroup(oauthProfileName, groupName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileAccessLevelGroup: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileaccesslevelgroupmsgvpnaccesslevelexception"></a>
# **DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException**
> SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName)

Delete a Message VPN Access-Level Exception object.

Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.

            try
            {
                // Delete a Message VPN Access-Level Exception object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException(oauthProfileName, groupName, msgVpnName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **msgVpnName** | **string**| The name of the message VPN. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileclientallowedhost"></a>
# **DeleteOauthProfileClientAllowedHost**
> SempMetaOnlyResponse DeleteOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost)

Delete an Allowed Host Value object.

Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileClientAllowedHostExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var allowedHost = allowedHost_example;  // string | An allowed value for the Host header.

            try
            {
                // Delete an Allowed Host Value object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileClientAllowedHost(oauthProfileName, allowedHost);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileClientAllowedHost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **allowedHost** | **string**| An allowed value for the Host header. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileclientauthorizationparameter"></a>
# **DeleteOauthProfileClientAuthorizationParameter**
> SempMetaOnlyResponse DeleteOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName)

Delete an Authorization Parameter object.

Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileClientAuthorizationParameterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var authorizationParameterName = authorizationParameterName_example;  // string | The name of the authorization parameter.

            try
            {
                // Delete an Authorization Parameter object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileClientAuthorizationParameter(oauthProfileName, authorizationParameterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileClientAuthorizationParameter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **authorizationParameterName** | **string**| The name of the authorization parameter. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileclientrequiredclaim"></a>
# **DeleteOauthProfileClientRequiredClaim**
> SempMetaOnlyResponse DeleteOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName)

Delete a Required Claim object.

Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var clientRequiredClaimName = clientRequiredClaimName_example;  // string | The name of the ID token claim to verify.

            try
            {
                // Delete a Required Claim object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileClientRequiredClaim(oauthProfileName, clientRequiredClaimName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **clientRequiredClaimName** | **string**| The name of the ID token claim to verify. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofiledefaultmsgvpnaccesslevelexception"></a>
# **DeleteOauthProfileDefaultMsgVpnAccessLevelException**
> SempMetaOnlyResponse DeleteOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName)

Delete a Message VPN Access-Level Exception object.

Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.

            try
            {
                // Delete a Message VPN Access-Level Exception object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileDefaultMsgVpnAccessLevelException(oauthProfileName, msgVpnName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileDefaultMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **msgVpnName** | **string**| The name of the message VPN. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteoauthprofileresourceserverrequiredclaim"></a>
# **DeleteOauthProfileResourceServerRequiredClaim**
> SempMetaOnlyResponse DeleteOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName)

Delete a Required Claim object.

Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var resourceServerRequiredClaimName = resourceServerRequiredClaimName_example;  // string | The name of the access token claim to verify.

            try
            {
                // Delete a Required Claim object.
                SempMetaOnlyResponse result = apiInstance.DeleteOauthProfileResourceServerRequiredClaim(oauthProfileName, resourceServerRequiredClaimName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.DeleteOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **resourceServerRequiredClaimName** | **string**| The name of the access token claim to verify. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofile"></a>
# **GetOauthProfile**
> OauthProfileResponse GetOauthProfile (string oauthProfileName, string opaquePassword = null, List<string> select = null)

Get an OAuth Profile object.

Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an OAuth Profile object.
                OauthProfileResponse result = apiInstance.GetOauthProfile(oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResponse**](OauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileaccesslevelgroup"></a>
# **GetOauthProfileAccessLevelGroup**
> OauthProfileAccessLevelGroupResponse GetOauthProfileAccessLevelGroup (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)

Get a Group Access Level object.

Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileAccessLevelGroupExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Group Access Level object.
                OauthProfileAccessLevelGroupResponse result = apiInstance.GetOauthProfileAccessLevelGroup(oauthProfileName, groupName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileAccessLevelGroup: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupResponse**](OauthProfileAccessLevelGroupResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileaccesslevelgroupmsgvpnaccesslevelexception"></a>
# **GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException**
> OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Get a Message VPN Access-Level Exception object.

Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Message VPN Access-Level Exception object.
                OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse result = apiInstance.GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException(oauthProfileName, groupName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse**](OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileaccesslevelgroupmsgvpnaccesslevelexceptions"></a>
# **GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions**
> OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Message VPN Access-Level Exception objects.

Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Message VPN Access-Level Exception objects.
                OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse result = apiInstance.GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions(oauthProfileName, groupName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse**](OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileaccesslevelgroups"></a>
# **GetOauthProfileAccessLevelGroups**
> OauthProfileAccessLevelGroupsResponse GetOauthProfileAccessLevelGroups (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Group Access Level objects.

Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileAccessLevelGroupsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Group Access Level objects.
                OauthProfileAccessLevelGroupsResponse result = apiInstance.GetOauthProfileAccessLevelGroups(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileAccessLevelGroups: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupsResponse**](OauthProfileAccessLevelGroupsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientallowedhost"></a>
# **GetOauthProfileClientAllowedHost**
> OauthProfileClientAllowedHostResponse GetOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null)

Get an Allowed Host Value object.

Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientAllowedHostExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var allowedHost = allowedHost_example;  // string | An allowed value for the Host header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an Allowed Host Value object.
                OauthProfileClientAllowedHostResponse result = apiInstance.GetOauthProfileClientAllowedHost(oauthProfileName, allowedHost, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientAllowedHost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **allowedHost** | **string**| An allowed value for the Host header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAllowedHostResponse**](OauthProfileClientAllowedHostResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientallowedhosts"></a>
# **GetOauthProfileClientAllowedHosts**
> OauthProfileClientAllowedHostsResponse GetOauthProfileClientAllowedHosts (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Allowed Host Value objects.

Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientAllowedHostsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Allowed Host Value objects.
                OauthProfileClientAllowedHostsResponse result = apiInstance.GetOauthProfileClientAllowedHosts(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientAllowedHosts: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAllowedHostsResponse**](OauthProfileClientAllowedHostsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientauthorizationparameter"></a>
# **GetOauthProfileClientAuthorizationParameter**
> OauthProfileClientAuthorizationParameterResponse GetOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)

Get an Authorization Parameter object.

Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientAuthorizationParameterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var authorizationParameterName = authorizationParameterName_example;  // string | The name of the authorization parameter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an Authorization Parameter object.
                OauthProfileClientAuthorizationParameterResponse result = apiInstance.GetOauthProfileClientAuthorizationParameter(oauthProfileName, authorizationParameterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientAuthorizationParameter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **authorizationParameterName** | **string**| The name of the authorization parameter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAuthorizationParameterResponse**](OauthProfileClientAuthorizationParameterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientauthorizationparameters"></a>
# **GetOauthProfileClientAuthorizationParameters**
> OauthProfileClientAuthorizationParametersResponse GetOauthProfileClientAuthorizationParameters (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Authorization Parameter objects.

Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientAuthorizationParametersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Authorization Parameter objects.
                OauthProfileClientAuthorizationParametersResponse result = apiInstance.GetOauthProfileClientAuthorizationParameters(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientAuthorizationParameters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAuthorizationParametersResponse**](OauthProfileClientAuthorizationParametersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientrequiredclaim"></a>
# **GetOauthProfileClientRequiredClaim**
> OauthProfileClientRequiredClaimResponse GetOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)

Get a Required Claim object.

Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var clientRequiredClaimName = clientRequiredClaimName_example;  // string | The name of the ID token claim to verify.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Required Claim object.
                OauthProfileClientRequiredClaimResponse result = apiInstance.GetOauthProfileClientRequiredClaim(oauthProfileName, clientRequiredClaimName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **clientRequiredClaimName** | **string**| The name of the ID token claim to verify. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientRequiredClaimResponse**](OauthProfileClientRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileclientrequiredclaims"></a>
# **GetOauthProfileClientRequiredClaims**
> OauthProfileClientRequiredClaimsResponse GetOauthProfileClientRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Required Claim objects.

Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileClientRequiredClaimsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Required Claim objects.
                OauthProfileClientRequiredClaimsResponse result = apiInstance.GetOauthProfileClientRequiredClaims(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileClientRequiredClaims: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientRequiredClaimsResponse**](OauthProfileClientRequiredClaimsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofiledefaultmsgvpnaccesslevelexception"></a>
# **GetOauthProfileDefaultMsgVpnAccessLevelException**
> OauthProfileDefaultMsgVpnAccessLevelExceptionResponse GetOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Get a Message VPN Access-Level Exception object.

Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileDefaultMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Message VPN Access-Level Exception object.
                OauthProfileDefaultMsgVpnAccessLevelExceptionResponse result = apiInstance.GetOauthProfileDefaultMsgVpnAccessLevelException(oauthProfileName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileDefaultMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileDefaultMsgVpnAccessLevelExceptionResponse**](OauthProfileDefaultMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofiledefaultmsgvpnaccesslevelexceptions"></a>
# **GetOauthProfileDefaultMsgVpnAccessLevelExceptions**
> OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse GetOauthProfileDefaultMsgVpnAccessLevelExceptions (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Message VPN Access-Level Exception objects.

Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileDefaultMsgVpnAccessLevelExceptionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Message VPN Access-Level Exception objects.
                OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse result = apiInstance.GetOauthProfileDefaultMsgVpnAccessLevelExceptions(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileDefaultMsgVpnAccessLevelExceptions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse**](OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileresourceserverrequiredclaim"></a>
# **GetOauthProfileResourceServerRequiredClaim**
> OauthProfileResourceServerRequiredClaimResponse GetOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)

Get a Required Claim object.

Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var resourceServerRequiredClaimName = resourceServerRequiredClaimName_example;  // string | The name of the access token claim to verify.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Required Claim object.
                OauthProfileResourceServerRequiredClaimResponse result = apiInstance.GetOauthProfileResourceServerRequiredClaim(oauthProfileName, resourceServerRequiredClaimName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **resourceServerRequiredClaimName** | **string**| The name of the access token claim to verify. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResourceServerRequiredClaimResponse**](OauthProfileResourceServerRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofileresourceserverrequiredclaims"></a>
# **GetOauthProfileResourceServerRequiredClaims**
> OauthProfileResourceServerRequiredClaimsResponse GetOauthProfileResourceServerRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Required Claim objects.

Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfileResourceServerRequiredClaimsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Required Claim objects.
                OauthProfileResourceServerRequiredClaimsResponse result = apiInstance.GetOauthProfileResourceServerRequiredClaims(oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfileResourceServerRequiredClaims: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResourceServerRequiredClaimsResponse**](OauthProfileResourceServerRequiredClaimsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getoauthprofiles"></a>
# **GetOauthProfiles**
> OauthProfilesResponse GetOauthProfiles (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of OAuth Profile objects.

Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetOauthProfilesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of OAuth Profile objects.
                OauthProfilesResponse result = apiInstance.GetOauthProfiles(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.GetOauthProfiles: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfilesResponse**](OauthProfilesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceoauthprofile"></a>
# **ReplaceOauthProfile**
> OauthProfileResponse ReplaceOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Replace an OAuth Profile object.

Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfile(); // OauthProfile | The OAuth Profile object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace an OAuth Profile object.
                OauthProfileResponse result = apiInstance.ReplaceOauthProfile(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.ReplaceOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfile**](OauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResponse**](OauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceoauthprofileaccesslevelgroup"></a>
# **ReplaceOauthProfileAccessLevelGroup**
> OauthProfileAccessLevelGroupResponse ReplaceOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)

Replace a Group Access Level object.

Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceOauthProfileAccessLevelGroupExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroup(); // OauthProfileAccessLevelGroup | The Group Access Level object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Group Access Level object.
                OauthProfileAccessLevelGroupResponse result = apiInstance.ReplaceOauthProfileAccessLevelGroup(body, oauthProfileName, groupName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.ReplaceOauthProfileAccessLevelGroup: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroup**](OauthProfileAccessLevelGroup.md)| The Group Access Level object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupResponse**](OauthProfileAccessLevelGroupResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceoauthprofileaccesslevelgroupmsgvpnaccesslevelexception"></a>
# **ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException**
> OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Replace a Message VPN Access-Level Exception object.

Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroupMsgVpnAccessLevelException(); // OauthProfileAccessLevelGroupMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Message VPN Access-Level Exception object.
                OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse result = apiInstance.ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileAccessLevelGroupMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse**](OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceoauthprofileclientauthorizationparameter"></a>
# **ReplaceOauthProfileClientAuthorizationParameter**
> OauthProfileClientAuthorizationParameterResponse ReplaceOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)

Replace an Authorization Parameter object.

Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceOauthProfileClientAuthorizationParameterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileClientAuthorizationParameter(); // OauthProfileClientAuthorizationParameter | The Authorization Parameter object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var authorizationParameterName = authorizationParameterName_example;  // string | The name of the authorization parameter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace an Authorization Parameter object.
                OauthProfileClientAuthorizationParameterResponse result = apiInstance.ReplaceOauthProfileClientAuthorizationParameter(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.ReplaceOauthProfileClientAuthorizationParameter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileClientAuthorizationParameter**](OauthProfileClientAuthorizationParameter.md)| The Authorization Parameter object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **authorizationParameterName** | **string**| The name of the authorization parameter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAuthorizationParameterResponse**](OauthProfileClientAuthorizationParameterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceoauthprofiledefaultmsgvpnaccesslevelexception"></a>
# **ReplaceOauthProfileDefaultMsgVpnAccessLevelException**
> OauthProfileDefaultMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Replace a Message VPN Access-Level Exception object.

Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileDefaultMsgVpnAccessLevelException(); // OauthProfileDefaultMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Message VPN Access-Level Exception object.
                OauthProfileDefaultMsgVpnAccessLevelExceptionResponse result = apiInstance.ReplaceOauthProfileDefaultMsgVpnAccessLevelException(body, oauthProfileName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.ReplaceOauthProfileDefaultMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileDefaultMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileDefaultMsgVpnAccessLevelExceptionResponse**](OauthProfileDefaultMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateoauthprofile"></a>
# **UpdateOauthProfile**
> OauthProfileResponse UpdateOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Update an OAuth Profile object.

Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfile(); // OauthProfile | The OAuth Profile object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update an OAuth Profile object.
                OauthProfileResponse result = apiInstance.UpdateOauthProfile(body, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.UpdateOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfile**](OauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileResponse**](OauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateoauthprofileaccesslevelgroup"></a>
# **UpdateOauthProfileAccessLevelGroup**
> OauthProfileAccessLevelGroupResponse UpdateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)

Update a Group Access Level object.

Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateOauthProfileAccessLevelGroupExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroup(); // OauthProfileAccessLevelGroup | The Group Access Level object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Group Access Level object.
                OauthProfileAccessLevelGroupResponse result = apiInstance.UpdateOauthProfileAccessLevelGroup(body, oauthProfileName, groupName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.UpdateOauthProfileAccessLevelGroup: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroup**](OauthProfileAccessLevelGroup.md)| The Group Access Level object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupResponse**](OauthProfileAccessLevelGroupResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateoauthprofileaccesslevelgroupmsgvpnaccesslevelexception"></a>
# **UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**
> OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Update a Message VPN Access-Level Exception object.

Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileAccessLevelGroupMsgVpnAccessLevelException(); // OauthProfileAccessLevelGroupMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var groupName = groupName_example;  // string | The name of the group.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Message VPN Access-Level Exception object.
                OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse result = apiInstance.UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileAccessLevelGroupMsgVpnAccessLevelException**](OauthProfileAccessLevelGroupMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **groupName** | **string**| The name of the group. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse**](OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateoauthprofileclientauthorizationparameter"></a>
# **UpdateOauthProfileClientAuthorizationParameter**
> OauthProfileClientAuthorizationParameterResponse UpdateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)

Update an Authorization Parameter object.

Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateOauthProfileClientAuthorizationParameterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileClientAuthorizationParameter(); // OauthProfileClientAuthorizationParameter | The Authorization Parameter object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var authorizationParameterName = authorizationParameterName_example;  // string | The name of the authorization parameter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update an Authorization Parameter object.
                OauthProfileClientAuthorizationParameterResponse result = apiInstance.UpdateOauthProfileClientAuthorizationParameter(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.UpdateOauthProfileClientAuthorizationParameter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileClientAuthorizationParameter**](OauthProfileClientAuthorizationParameter.md)| The Authorization Parameter object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **authorizationParameterName** | **string**| The name of the authorization parameter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileClientAuthorizationParameterResponse**](OauthProfileClientAuthorizationParameterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateoauthprofiledefaultmsgvpnaccesslevelexception"></a>
# **UpdateOauthProfileDefaultMsgVpnAccessLevelException**
> OauthProfileDefaultMsgVpnAccessLevelExceptionResponse UpdateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)

Update a Message VPN Access-Level Exception object.

Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.24.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new OauthProfileApi();
            var body = new OauthProfileDefaultMsgVpnAccessLevelException(); // OauthProfileDefaultMsgVpnAccessLevelException | The Message VPN Access-Level Exception object's attributes.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var msgVpnName = msgVpnName_example;  // string | The name of the message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Message VPN Access-Level Exception object.
                OauthProfileDefaultMsgVpnAccessLevelExceptionResponse result = apiInstance.UpdateOauthProfileDefaultMsgVpnAccessLevelException(body, oauthProfileName, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling OauthProfileApi.UpdateOauthProfileDefaultMsgVpnAccessLevelException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**OauthProfileDefaultMsgVpnAccessLevelException**](OauthProfileDefaultMsgVpnAccessLevelException.md)| The Message VPN Access-Level Exception object&#x27;s attributes. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **msgVpnName** | **string**| The name of the message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**OauthProfileDefaultMsgVpnAccessLevelExceptionResponse**](OauthProfileDefaultMsgVpnAccessLevelExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
