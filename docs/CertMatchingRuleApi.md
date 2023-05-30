# Semp.V2.CSharp.Api.CertMatchingRuleApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnCertMatchingRule**](CertMatchingRuleApi.md#createmsgvpncertmatchingrule) | **POST** /msgVpns/{msgVpnName}/certMatchingRules | Create a Certificate Matching Rule object.
[**CreateMsgVpnCertMatchingRuleAttributeFilter**](CertMatchingRuleApi.md#createmsgvpncertmatchingruleattributefilter) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
[**CreateMsgVpnCertMatchingRuleCondition**](CertMatchingRuleApi.md#createmsgvpncertmatchingrulecondition) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
[**DeleteMsgVpnCertMatchingRule**](CertMatchingRuleApi.md#deletemsgvpncertmatchingrule) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
[**DeleteMsgVpnCertMatchingRuleAttributeFilter**](CertMatchingRuleApi.md#deletemsgvpncertmatchingruleattributefilter) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
[**DeleteMsgVpnCertMatchingRuleCondition**](CertMatchingRuleApi.md#deletemsgvpncertmatchingrulecondition) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
[**GetMsgVpnCertMatchingRule**](CertMatchingRuleApi.md#getmsgvpncertmatchingrule) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
[**GetMsgVpnCertMatchingRuleAttributeFilter**](CertMatchingRuleApi.md#getmsgvpncertmatchingruleattributefilter) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
[**GetMsgVpnCertMatchingRuleAttributeFilters**](CertMatchingRuleApi.md#getmsgvpncertmatchingruleattributefilters) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
[**GetMsgVpnCertMatchingRuleCondition**](CertMatchingRuleApi.md#getmsgvpncertmatchingrulecondition) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
[**GetMsgVpnCertMatchingRuleConditions**](CertMatchingRuleApi.md#getmsgvpncertmatchingruleconditions) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
[**GetMsgVpnCertMatchingRules**](CertMatchingRuleApi.md#getmsgvpncertmatchingrules) | **GET** /msgVpns/{msgVpnName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
[**ReplaceMsgVpnCertMatchingRule**](CertMatchingRuleApi.md#replacemsgvpncertmatchingrule) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
[**ReplaceMsgVpnCertMatchingRuleAttributeFilter**](CertMatchingRuleApi.md#replacemsgvpncertmatchingruleattributefilter) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
[**UpdateMsgVpnCertMatchingRule**](CertMatchingRuleApi.md#updatemsgvpncertmatchingrule) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
[**UpdateMsgVpnCertMatchingRuleAttributeFilter**](CertMatchingRuleApi.md#updatemsgvpncertmatchingruleattributefilter) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.

<a name="createmsgvpncertmatchingrule"></a>
# **CreateMsgVpnCertMatchingRule**
> MsgVpnCertMatchingRuleResponse CreateMsgVpnCertMatchingRule (MsgVpnCertMatchingRule body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule object.

Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRule(); // MsgVpnCertMatchingRule | The Certificate Matching Rule object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule object.
                MsgVpnCertMatchingRuleResponse result = apiInstance.CreateMsgVpnCertMatchingRule(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.CreateMsgVpnCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRule**](MsgVpnCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleResponse**](MsgVpnCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpncertmatchingruleattributefilter"></a>
# **CreateMsgVpnCertMatchingRuleAttributeFilter**
> MsgVpnCertMatchingRuleAttributeFilterResponse CreateMsgVpnCertMatchingRuleAttributeFilter (MsgVpnCertMatchingRuleAttributeFilter body, string msgVpnName, string ruleName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule Attribute Filter object.

Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: filterName|x|x|||| msgVpnName|x||x||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRuleAttributeFilter(); // MsgVpnCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule Attribute Filter object.
                MsgVpnCertMatchingRuleAttributeFilterResponse result = apiInstance.CreateMsgVpnCertMatchingRuleAttributeFilter(body, msgVpnName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.CreateMsgVpnCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRuleAttributeFilter**](MsgVpnCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleAttributeFilterResponse**](MsgVpnCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpncertmatchingrulecondition"></a>
# **CreateMsgVpnCertMatchingRuleCondition**
> MsgVpnCertMatchingRuleConditionResponse CreateMsgVpnCertMatchingRuleCondition (MsgVpnCertMatchingRuleCondition body, string msgVpnName, string ruleName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule Condition object.

Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a username attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRuleCondition(); // MsgVpnCertMatchingRuleCondition | The Certificate Matching Rule Condition object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule Condition object.
                MsgVpnCertMatchingRuleConditionResponse result = apiInstance.CreateMsgVpnCertMatchingRuleCondition(body, msgVpnName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.CreateMsgVpnCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRuleCondition**](MsgVpnCertMatchingRuleCondition.md)| The Certificate Matching Rule Condition object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleConditionResponse**](MsgVpnCertMatchingRuleConditionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpncertmatchingrule"></a>
# **DeleteMsgVpnCertMatchingRule**
> SempMetaOnlyResponse DeleteMsgVpnCertMatchingRule (string msgVpnName, string ruleName)

Delete a Certificate Matching Rule object.

Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.

            try
            {
                // Delete a Certificate Matching Rule object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnCertMatchingRule(msgVpnName, ruleName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.DeleteMsgVpnCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpncertmatchingruleattributefilter"></a>
# **DeleteMsgVpnCertMatchingRuleAttributeFilter**
> SempMetaOnlyResponse DeleteMsgVpnCertMatchingRuleAttributeFilter (string msgVpnName, string ruleName, string filterName)

Delete a Certificate Matching Rule Attribute Filter object.

Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.

            try
            {
                // Delete a Certificate Matching Rule Attribute Filter object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnCertMatchingRuleAttributeFilter(msgVpnName, ruleName, filterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.DeleteMsgVpnCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpncertmatchingrulecondition"></a>
# **DeleteMsgVpnCertMatchingRuleCondition**
> SempMetaOnlyResponse DeleteMsgVpnCertMatchingRuleCondition (string msgVpnName, string ruleName, string source)

Delete a Certificate Matching Rule Condition object.

Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a username attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var source = source_example;  // string | Certificate field to be compared with the Attribute.

            try
            {
                // Delete a Certificate Matching Rule Condition object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnCertMatchingRuleCondition(msgVpnName, ruleName, source);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.DeleteMsgVpnCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **source** | **string**| Certificate field to be compared with the Attribute. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingrule"></a>
# **GetMsgVpnCertMatchingRule**
> MsgVpnCertMatchingRuleResponse GetMsgVpnCertMatchingRule (string msgVpnName, string ruleName, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule object.

Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule object.
                MsgVpnCertMatchingRuleResponse result = apiInstance.GetMsgVpnCertMatchingRule(msgVpnName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleResponse**](MsgVpnCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingruleattributefilter"></a>
# **GetMsgVpnCertMatchingRuleAttributeFilter**
> MsgVpnCertMatchingRuleAttributeFilterResponse GetMsgVpnCertMatchingRuleAttributeFilter (string msgVpnName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule Attribute Filter object.

Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: filterName|x||| msgVpnName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule Attribute Filter object.
                MsgVpnCertMatchingRuleAttributeFilterResponse result = apiInstance.GetMsgVpnCertMatchingRuleAttributeFilter(msgVpnName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleAttributeFilterResponse**](MsgVpnCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingruleattributefilters"></a>
# **GetMsgVpnCertMatchingRuleAttributeFilters**
> MsgVpnCertMatchingRuleAttributeFiltersResponse GetMsgVpnCertMatchingRuleAttributeFilters (string msgVpnName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule Attribute Filter objects.

Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: filterName|x||| msgVpnName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRuleAttributeFiltersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule Attribute Filter objects.
                MsgVpnCertMatchingRuleAttributeFiltersResponse result = apiInstance.GetMsgVpnCertMatchingRuleAttributeFilters(msgVpnName, ruleName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRuleAttributeFilters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleAttributeFiltersResponse**](MsgVpnCertMatchingRuleAttributeFiltersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingrulecondition"></a>
# **GetMsgVpnCertMatchingRuleCondition**
> MsgVpnCertMatchingRuleConditionResponse GetMsgVpnCertMatchingRuleCondition (string msgVpnName, string ruleName, string source, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule Condition object.

Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a username attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var source = source_example;  // string | Certificate field to be compared with the Attribute.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule Condition object.
                MsgVpnCertMatchingRuleConditionResponse result = apiInstance.GetMsgVpnCertMatchingRuleCondition(msgVpnName, ruleName, source, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **source** | **string**| Certificate field to be compared with the Attribute. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleConditionResponse**](MsgVpnCertMatchingRuleConditionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingruleconditions"></a>
# **GetMsgVpnCertMatchingRuleConditions**
> MsgVpnCertMatchingRuleConditionsResponse GetMsgVpnCertMatchingRuleConditions (string msgVpnName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule Condition objects.

Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a username attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRuleConditionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule Condition objects.
                MsgVpnCertMatchingRuleConditionsResponse result = apiInstance.GetMsgVpnCertMatchingRuleConditions(msgVpnName, ruleName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRuleConditions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleConditionsResponse**](MsgVpnCertMatchingRuleConditionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpncertmatchingrules"></a>
# **GetMsgVpnCertMatchingRules**
> MsgVpnCertMatchingRulesResponse GetMsgVpnCertMatchingRules (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule objects.

Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnCertMatchingRulesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule objects.
                MsgVpnCertMatchingRulesResponse result = apiInstance.GetMsgVpnCertMatchingRules(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.GetMsgVpnCertMatchingRules: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRulesResponse**](MsgVpnCertMatchingRulesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpncertmatchingrule"></a>
# **ReplaceMsgVpnCertMatchingRule**
> MsgVpnCertMatchingRuleResponse ReplaceMsgVpnCertMatchingRule (MsgVpnCertMatchingRule body, string msgVpnName, string ruleName, string opaquePassword = null, List<string> select = null)

Replace a Certificate Matching Rule object.

Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRule(); // MsgVpnCertMatchingRule | The Certificate Matching Rule object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Certificate Matching Rule object.
                MsgVpnCertMatchingRuleResponse result = apiInstance.ReplaceMsgVpnCertMatchingRule(body, msgVpnName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.ReplaceMsgVpnCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRule**](MsgVpnCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleResponse**](MsgVpnCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpncertmatchingruleattributefilter"></a>
# **ReplaceMsgVpnCertMatchingRuleAttributeFilter**
> MsgVpnCertMatchingRuleAttributeFilterResponse ReplaceMsgVpnCertMatchingRuleAttributeFilter (MsgVpnCertMatchingRuleAttributeFilter body, string msgVpnName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Replace a Certificate Matching Rule Attribute Filter object.

Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- filterName|x||x||||| msgVpnName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRuleAttributeFilter(); // MsgVpnCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Certificate Matching Rule Attribute Filter object.
                MsgVpnCertMatchingRuleAttributeFilterResponse result = apiInstance.ReplaceMsgVpnCertMatchingRuleAttributeFilter(body, msgVpnName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.ReplaceMsgVpnCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRuleAttributeFilter**](MsgVpnCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleAttributeFilterResponse**](MsgVpnCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpncertmatchingrule"></a>
# **UpdateMsgVpnCertMatchingRule**
> MsgVpnCertMatchingRuleResponse UpdateMsgVpnCertMatchingRule (MsgVpnCertMatchingRule body, string msgVpnName, string ruleName, string opaquePassword = null, List<string> select = null)

Update a Certificate Matching Rule object.

Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given username.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.27.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRule(); // MsgVpnCertMatchingRule | The Certificate Matching Rule object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Certificate Matching Rule object.
                MsgVpnCertMatchingRuleResponse result = apiInstance.UpdateMsgVpnCertMatchingRule(body, msgVpnName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.UpdateMsgVpnCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRule**](MsgVpnCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleResponse**](MsgVpnCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpncertmatchingruleattributefilter"></a>
# **UpdateMsgVpnCertMatchingRuleAttributeFilter**
> MsgVpnCertMatchingRuleAttributeFilterResponse UpdateMsgVpnCertMatchingRuleAttributeFilter (MsgVpnCertMatchingRuleAttributeFilter body, string msgVpnName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Update a Certificate Matching Rule Attribute Filter object.

Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a username attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- filterName|x|x||||| msgVpnName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertMatchingRuleApi();
            var body = new MsgVpnCertMatchingRuleAttributeFilter(); // MsgVpnCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Certificate Matching Rule Attribute Filter object.
                MsgVpnCertMatchingRuleAttributeFilterResponse result = apiInstance.UpdateMsgVpnCertMatchingRuleAttributeFilter(body, msgVpnName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertMatchingRuleApi.UpdateMsgVpnCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnCertMatchingRuleAttributeFilter**](MsgVpnCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnCertMatchingRuleAttributeFilterResponse**](MsgVpnCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
