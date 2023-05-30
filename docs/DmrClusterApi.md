# Semp.V2.CSharp.Api.DmrClusterApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateDmrCluster**](DmrClusterApi.md#createdmrcluster) | **POST** /dmrClusters | Create a Cluster object.
[**CreateDmrClusterCertMatchingRule**](DmrClusterApi.md#createdmrclustercertmatchingrule) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules | Create a Certificate Matching Rule object.
[**CreateDmrClusterCertMatchingRuleAttributeFilter**](DmrClusterApi.md#createdmrclustercertmatchingruleattributefilter) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
[**CreateDmrClusterCertMatchingRuleCondition**](DmrClusterApi.md#createdmrclustercertmatchingrulecondition) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
[**CreateDmrClusterLink**](DmrClusterApi.md#createdmrclusterlink) | **POST** /dmrClusters/{dmrClusterName}/links | Create a Link object.
[**CreateDmrClusterLinkAttribute**](DmrClusterApi.md#createdmrclusterlinkattribute) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Create a Link Attribute object.
[**CreateDmrClusterLinkRemoteAddress**](DmrClusterApi.md#createdmrclusterlinkremoteaddress) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Create a Remote Address object.
[**CreateDmrClusterLinkTlsTrustedCommonName**](DmrClusterApi.md#createdmrclusterlinktlstrustedcommonname) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
[**DeleteDmrCluster**](DmrClusterApi.md#deletedmrcluster) | **DELETE** /dmrClusters/{dmrClusterName} | Delete a Cluster object.
[**DeleteDmrClusterCertMatchingRule**](DmrClusterApi.md#deletedmrclustercertmatchingrule) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
[**DeleteDmrClusterCertMatchingRuleAttributeFilter**](DmrClusterApi.md#deletedmrclustercertmatchingruleattributefilter) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
[**DeleteDmrClusterCertMatchingRuleCondition**](DmrClusterApi.md#deletedmrclustercertmatchingrulecondition) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
[**DeleteDmrClusterLink**](DmrClusterApi.md#deletedmrclusterlink) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Delete a Link object.
[**DeleteDmrClusterLinkAttribute**](DmrClusterApi.md#deletedmrclusterlinkattribute) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Delete a Link Attribute object.
[**DeleteDmrClusterLinkRemoteAddress**](DmrClusterApi.md#deletedmrclusterlinkremoteaddress) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Delete a Remote Address object.
[**DeleteDmrClusterLinkTlsTrustedCommonName**](DmrClusterApi.md#deletedmrclusterlinktlstrustedcommonname) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
[**GetDmrCluster**](DmrClusterApi.md#getdmrcluster) | **GET** /dmrClusters/{dmrClusterName} | Get a Cluster object.
[**GetDmrClusterCertMatchingRule**](DmrClusterApi.md#getdmrclustercertmatchingrule) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
[**GetDmrClusterCertMatchingRuleAttributeFilter**](DmrClusterApi.md#getdmrclustercertmatchingruleattributefilter) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
[**GetDmrClusterCertMatchingRuleAttributeFilters**](DmrClusterApi.md#getdmrclustercertmatchingruleattributefilters) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
[**GetDmrClusterCertMatchingRuleCondition**](DmrClusterApi.md#getdmrclustercertmatchingrulecondition) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
[**GetDmrClusterCertMatchingRuleConditions**](DmrClusterApi.md#getdmrclustercertmatchingruleconditions) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
[**GetDmrClusterCertMatchingRules**](DmrClusterApi.md#getdmrclustercertmatchingrules) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
[**GetDmrClusterLink**](DmrClusterApi.md#getdmrclusterlink) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Get a Link object.
[**GetDmrClusterLinkAttribute**](DmrClusterApi.md#getdmrclusterlinkattribute) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Get a Link Attribute object.
[**GetDmrClusterLinkAttributes**](DmrClusterApi.md#getdmrclusterlinkattributes) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Get a list of Link Attribute objects.
[**GetDmrClusterLinkRemoteAddress**](DmrClusterApi.md#getdmrclusterlinkremoteaddress) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Get a Remote Address object.
[**GetDmrClusterLinkRemoteAddresses**](DmrClusterApi.md#getdmrclusterlinkremoteaddresses) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Get a list of Remote Address objects.
[**GetDmrClusterLinkTlsTrustedCommonName**](DmrClusterApi.md#getdmrclusterlinktlstrustedcommonname) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
[**GetDmrClusterLinkTlsTrustedCommonNames**](DmrClusterApi.md#getdmrclusterlinktlstrustedcommonnames) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
[**GetDmrClusterLinks**](DmrClusterApi.md#getdmrclusterlinks) | **GET** /dmrClusters/{dmrClusterName}/links | Get a list of Link objects.
[**GetDmrClusters**](DmrClusterApi.md#getdmrclusters) | **GET** /dmrClusters | Get a list of Cluster objects.
[**ReplaceDmrCluster**](DmrClusterApi.md#replacedmrcluster) | **PUT** /dmrClusters/{dmrClusterName} | Replace a Cluster object.
[**ReplaceDmrClusterCertMatchingRule**](DmrClusterApi.md#replacedmrclustercertmatchingrule) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
[**ReplaceDmrClusterCertMatchingRuleAttributeFilter**](DmrClusterApi.md#replacedmrclustercertmatchingruleattributefilter) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
[**ReplaceDmrClusterLink**](DmrClusterApi.md#replacedmrclusterlink) | **PUT** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Replace a Link object.
[**UpdateDmrCluster**](DmrClusterApi.md#updatedmrcluster) | **PATCH** /dmrClusters/{dmrClusterName} | Update a Cluster object.
[**UpdateDmrClusterCertMatchingRule**](DmrClusterApi.md#updatedmrclustercertmatchingrule) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
[**UpdateDmrClusterCertMatchingRuleAttributeFilter**](DmrClusterApi.md#updatedmrclustercertmatchingruleattributefilter) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
[**UpdateDmrClusterLink**](DmrClusterApi.md#updatedmrclusterlink) | **PATCH** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Update a Link object.

<a name="createdmrcluster"></a>
# **CreateDmrCluster**
> DmrClusterResponse CreateDmrCluster (DmrCluster body, string opaquePassword = null, List<string> select = null)

Create a Cluster object.

Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrCluster(); // DmrCluster | The Cluster object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Cluster object.
                DmrClusterResponse result = apiInstance.CreateDmrCluster(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrCluster**](DmrCluster.md)| The Cluster object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterResponse**](DmrClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclustercertmatchingrule"></a>
# **CreateDmrClusterCertMatchingRule**
> DmrClusterCertMatchingRuleResponse CreateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule object.

Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRule(); // DmrClusterCertMatchingRule | The Certificate Matching Rule object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule object.
                DmrClusterCertMatchingRuleResponse result = apiInstance.CreateDmrClusterCertMatchingRule(body, dmrClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRule**](DmrClusterCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleResponse**](DmrClusterCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclustercertmatchingruleattributefilter"></a>
# **CreateDmrClusterCertMatchingRuleAttributeFilter**
> DmrClusterCertMatchingRuleAttributeFilterResponse CreateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule Attribute Filter object.

Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRuleAttributeFilter(); // DmrClusterCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule Attribute Filter object.
                DmrClusterCertMatchingRuleAttributeFilterResponse result = apiInstance.CreateDmrClusterCertMatchingRuleAttributeFilter(body, dmrClusterName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRuleAttributeFilter**](DmrClusterCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleAttributeFilterResponse**](DmrClusterCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclustercertmatchingrulecondition"></a>
# **CreateDmrClusterCertMatchingRuleCondition**
> DmrClusterCertMatchingRuleConditionResponse CreateDmrClusterCertMatchingRuleCondition (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)

Create a Certificate Matching Rule Condition object.

Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRuleCondition(); // DmrClusterCertMatchingRuleCondition | The Certificate Matching Rule Condition object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Matching Rule Condition object.
                DmrClusterCertMatchingRuleConditionResponse result = apiInstance.CreateDmrClusterCertMatchingRuleCondition(body, dmrClusterName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRuleCondition**](DmrClusterCertMatchingRuleCondition.md)| The Certificate Matching Rule Condition object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleConditionResponse**](DmrClusterCertMatchingRuleConditionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclusterlink"></a>
# **CreateDmrClusterLink**
> DmrClusterLinkResponse CreateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null)

Create a Link object.

Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterLinkExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLink(); // DmrClusterLink | The Link object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Link object.
                DmrClusterLinkResponse result = apiInstance.CreateDmrClusterLink(body, dmrClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterLink: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLink**](DmrClusterLink.md)| The Link object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkResponse**](DmrClusterLinkResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclusterlinkattribute"></a>
# **CreateDmrClusterLinkAttribute**
> DmrClusterLinkAttributeResponse CreateDmrClusterLinkAttribute (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Create a Link Attribute object.

Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterLinkAttributeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLinkAttribute(); // DmrClusterLinkAttribute | The Link Attribute object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Link Attribute object.
                DmrClusterLinkAttributeResponse result = apiInstance.CreateDmrClusterLinkAttribute(body, dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterLinkAttribute: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLinkAttribute**](DmrClusterLinkAttribute.md)| The Link Attribute object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkAttributeResponse**](DmrClusterLinkAttributeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclusterlinkremoteaddress"></a>
# **CreateDmrClusterLinkRemoteAddress**
> DmrClusterLinkRemoteAddressResponse CreateDmrClusterLinkRemoteAddress (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Create a Remote Address object.

Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterLinkRemoteAddressExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLinkRemoteAddress(); // DmrClusterLinkRemoteAddress | The Remote Address object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Remote Address object.
                DmrClusterLinkRemoteAddressResponse result = apiInstance.CreateDmrClusterLinkRemoteAddress(body, dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterLinkRemoteAddress: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLinkRemoteAddress**](DmrClusterLinkRemoteAddress.md)| The Remote Address object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkRemoteAddressResponse**](DmrClusterLinkRemoteAddressResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createdmrclusterlinktlstrustedcommonname"></a>
# **CreateDmrClusterLinkTlsTrustedCommonName**
> DmrClusterLinkTlsTrustedCommonNameResponse CreateDmrClusterLinkTlsTrustedCommonName (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Create a Trusted Common Name object.

Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDmrClusterLinkTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLinkTlsTrustedCommonName(); // DmrClusterLinkTlsTrustedCommonName | The Trusted Common Name object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Trusted Common Name object.
                DmrClusterLinkTlsTrustedCommonNameResponse result = apiInstance.CreateDmrClusterLinkTlsTrustedCommonName(body, dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.CreateDmrClusterLinkTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLinkTlsTrustedCommonName**](DmrClusterLinkTlsTrustedCommonName.md)| The Trusted Common Name object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkTlsTrustedCommonNameResponse**](DmrClusterLinkTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrcluster"></a>
# **DeleteDmrCluster**
> SempMetaOnlyResponse DeleteDmrCluster (string dmrClusterName)

Delete a Cluster object.

Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.

            try
            {
                // Delete a Cluster object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrCluster(dmrClusterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrclustercertmatchingrule"></a>
# **DeleteDmrClusterCertMatchingRule**
> SempMetaOnlyResponse DeleteDmrClusterCertMatchingRule (string dmrClusterName, string ruleName)

Delete a Certificate Matching Rule object.

Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.

            try
            {
                // Delete a Certificate Matching Rule object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterCertMatchingRule(dmrClusterName, ruleName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrclustercertmatchingruleattributefilter"></a>
# **DeleteDmrClusterCertMatchingRuleAttributeFilter**
> SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName)

Delete a Certificate Matching Rule Attribute Filter object.

Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.

            try
            {
                // Delete a Certificate Matching Rule Attribute Filter object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterCertMatchingRuleAttributeFilter(dmrClusterName, ruleName, filterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
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
<a name="deletedmrclustercertmatchingrulecondition"></a>
# **DeleteDmrClusterCertMatchingRuleCondition**
> SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source)

Delete a Certificate Matching Rule Condition object.

Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var source = source_example;  // string | Certificate field to be compared with the Attribute.

            try
            {
                // Delete a Certificate Matching Rule Condition object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterCertMatchingRuleCondition(dmrClusterName, ruleName, source);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
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
<a name="deletedmrclusterlink"></a>
# **DeleteDmrClusterLink**
> SempMetaOnlyResponse DeleteDmrClusterLink (string dmrClusterName, string remoteNodeName)

Delete a Link object.

Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterLinkExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.

            try
            {
                // Delete a Link object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterLink(dmrClusterName, remoteNodeName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterLink: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrclusterlinkattribute"></a>
# **DeleteDmrClusterLinkAttribute**
> SempMetaOnlyResponse DeleteDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue)

Delete a Link Attribute object.

Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterLinkAttributeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var attributeName = attributeName_example;  // string | The name of the Attribute.
            var attributeValue = attributeValue_example;  // string | The value of the Attribute.

            try
            {
                // Delete a Link Attribute object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterLinkAttribute(dmrClusterName, remoteNodeName, attributeName, attributeValue);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterLinkAttribute: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **attributeName** | **string**| The name of the Attribute. | 
 **attributeValue** | **string**| The value of the Attribute. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrclusterlinkremoteaddress"></a>
# **DeleteDmrClusterLinkRemoteAddress**
> SempMetaOnlyResponse DeleteDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress)

Delete a Remote Address object.

Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterLinkRemoteAddressExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var remoteAddress = remoteAddress_example;  // string | The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).

            try
            {
                // Delete a Remote Address object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterLinkRemoteAddress(dmrClusterName, remoteNodeName, remoteAddress);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterLinkRemoteAddress: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **remoteAddress** | **string**| The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed). | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedmrclusterlinktlstrustedcommonname"></a>
# **DeleteDmrClusterLinkTlsTrustedCommonName**
> SempMetaOnlyResponse DeleteDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName)

Delete a Trusted Common Name object.

Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDmrClusterLinkTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.

            try
            {
                // Delete a Trusted Common Name object.
                SempMetaOnlyResponse result = apiInstance.DeleteDmrClusterLinkTlsTrustedCommonName(dmrClusterName, remoteNodeName, tlsTrustedCommonName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.DeleteDmrClusterLinkTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrcluster"></a>
# **GetDmrCluster**
> DmrClusterResponse GetDmrCluster (string dmrClusterName, string opaquePassword = null, List<string> select = null)

Get a Cluster object.

Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Cluster object.
                DmrClusterResponse result = apiInstance.GetDmrCluster(dmrClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterResponse**](DmrClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingrule"></a>
# **GetDmrClusterCertMatchingRule**
> DmrClusterCertMatchingRuleResponse GetDmrClusterCertMatchingRule (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule object.

Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule object.
                DmrClusterCertMatchingRuleResponse result = apiInstance.GetDmrClusterCertMatchingRule(dmrClusterName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleResponse**](DmrClusterCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingruleattributefilter"></a>
# **GetDmrClusterCertMatchingRuleAttributeFilter**
> DmrClusterCertMatchingRuleAttributeFilterResponse GetDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule Attribute Filter object.

Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule Attribute Filter object.
                DmrClusterCertMatchingRuleAttributeFilterResponse result = apiInstance.GetDmrClusterCertMatchingRuleAttributeFilter(dmrClusterName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleAttributeFilterResponse**](DmrClusterCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingruleattributefilters"></a>
# **GetDmrClusterCertMatchingRuleAttributeFilters**
> DmrClusterCertMatchingRuleAttributeFiltersResponse GetDmrClusterCertMatchingRuleAttributeFilters (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule Attribute Filter objects.

Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRuleAttributeFiltersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule Attribute Filter objects.
                DmrClusterCertMatchingRuleAttributeFiltersResponse result = apiInstance.GetDmrClusterCertMatchingRuleAttributeFilters(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRuleAttributeFilters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleAttributeFiltersResponse**](DmrClusterCertMatchingRuleAttributeFiltersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingrulecondition"></a>
# **GetDmrClusterCertMatchingRuleCondition**
> DmrClusterCertMatchingRuleConditionResponse GetDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null)

Get a Certificate Matching Rule Condition object.

Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRuleConditionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var source = source_example;  // string | Certificate field to be compared with the Attribute.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Matching Rule Condition object.
                DmrClusterCertMatchingRuleConditionResponse result = apiInstance.GetDmrClusterCertMatchingRuleCondition(dmrClusterName, ruleName, source, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRuleCondition: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **source** | **string**| Certificate field to be compared with the Attribute. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleConditionResponse**](DmrClusterCertMatchingRuleConditionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingruleconditions"></a>
# **GetDmrClusterCertMatchingRuleConditions**
> DmrClusterCertMatchingRuleConditionsResponse GetDmrClusterCertMatchingRuleConditions (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule Condition objects.

Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRuleConditionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule Condition objects.
                DmrClusterCertMatchingRuleConditionsResponse result = apiInstance.GetDmrClusterCertMatchingRuleConditions(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRuleConditions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleConditionsResponse**](DmrClusterCertMatchingRuleConditionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclustercertmatchingrules"></a>
# **GetDmrClusterCertMatchingRules**
> DmrClusterCertMatchingRulesResponse GetDmrClusterCertMatchingRules (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Matching Rule objects.

Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterCertMatchingRulesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Matching Rule objects.
                DmrClusterCertMatchingRulesResponse result = apiInstance.GetDmrClusterCertMatchingRules(dmrClusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterCertMatchingRules: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRulesResponse**](DmrClusterCertMatchingRulesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlink"></a>
# **GetDmrClusterLink**
> DmrClusterLinkResponse GetDmrClusterLink (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Get a Link object.

Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Link object.
                DmrClusterLinkResponse result = apiInstance.GetDmrClusterLink(dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLink: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkResponse**](DmrClusterLinkResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinkattribute"></a>
# **GetDmrClusterLinkAttribute**
> DmrClusterLinkAttributeResponse GetDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null)

Get a Link Attribute object.

Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkAttributeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var attributeName = attributeName_example;  // string | The name of the Attribute.
            var attributeValue = attributeValue_example;  // string | The value of the Attribute.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Link Attribute object.
                DmrClusterLinkAttributeResponse result = apiInstance.GetDmrClusterLinkAttribute(dmrClusterName, remoteNodeName, attributeName, attributeValue, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkAttribute: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **attributeName** | **string**| The name of the Attribute. | 
 **attributeValue** | **string**| The value of the Attribute. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkAttributeResponse**](DmrClusterLinkAttributeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinkattributes"></a>
# **GetDmrClusterLinkAttributes**
> DmrClusterLinkAttributesResponse GetDmrClusterLinkAttributes (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Link Attribute objects.

Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkAttributesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Link Attribute objects.
                DmrClusterLinkAttributesResponse result = apiInstance.GetDmrClusterLinkAttributes(dmrClusterName, remoteNodeName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkAttributes: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkAttributesResponse**](DmrClusterLinkAttributesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinkremoteaddress"></a>
# **GetDmrClusterLinkRemoteAddress**
> DmrClusterLinkRemoteAddressResponse GetDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null)

Get a Remote Address object.

Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkRemoteAddressExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var remoteAddress = remoteAddress_example;  // string | The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Remote Address object.
                DmrClusterLinkRemoteAddressResponse result = apiInstance.GetDmrClusterLinkRemoteAddress(dmrClusterName, remoteNodeName, remoteAddress, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkRemoteAddress: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **remoteAddress** | **string**| The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed). | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkRemoteAddressResponse**](DmrClusterLinkRemoteAddressResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinkremoteaddresses"></a>
# **GetDmrClusterLinkRemoteAddresses**
> DmrClusterLinkRemoteAddressesResponse GetDmrClusterLinkRemoteAddresses (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Remote Address objects.

Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkRemoteAddressesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Remote Address objects.
                DmrClusterLinkRemoteAddressesResponse result = apiInstance.GetDmrClusterLinkRemoteAddresses(dmrClusterName, remoteNodeName, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkRemoteAddresses: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkRemoteAddressesResponse**](DmrClusterLinkRemoteAddressesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinktlstrustedcommonname"></a>
# **GetDmrClusterLinkTlsTrustedCommonName**
> DmrClusterLinkTlsTrustedCommonNameResponse GetDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)

Get a Trusted Common Name object.

Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Trusted Common Name object.
                DmrClusterLinkTlsTrustedCommonNameResponse result = apiInstance.GetDmrClusterLinkTlsTrustedCommonName(dmrClusterName, remoteNodeName, tlsTrustedCommonName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkTlsTrustedCommonNameResponse**](DmrClusterLinkTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinktlstrustedcommonnames"></a>
# **GetDmrClusterLinkTlsTrustedCommonNames**
> DmrClusterLinkTlsTrustedCommonNamesResponse GetDmrClusterLinkTlsTrustedCommonNames (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Trusted Common Name objects.

Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinkTlsTrustedCommonNamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Trusted Common Name objects.
                DmrClusterLinkTlsTrustedCommonNamesResponse result = apiInstance.GetDmrClusterLinkTlsTrustedCommonNames(dmrClusterName, remoteNodeName, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinkTlsTrustedCommonNames: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkTlsTrustedCommonNamesResponse**](DmrClusterLinkTlsTrustedCommonNamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusterlinks"></a>
# **GetDmrClusterLinks**
> DmrClusterLinksResponse GetDmrClusterLinks (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Link objects.

Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClusterLinksExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Link objects.
                DmrClusterLinksResponse result = apiInstance.GetDmrClusterLinks(dmrClusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusterLinks: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinksResponse**](DmrClusterLinksResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdmrclusters"></a>
# **GetDmrClusters**
> DmrClustersResponse GetDmrClusters (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Cluster objects.

Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDmrClustersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Cluster objects.
                DmrClustersResponse result = apiInstance.GetDmrClusters(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.GetDmrClusters: " + e.Message );
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

[**DmrClustersResponse**](DmrClustersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacedmrcluster"></a>
# **ReplaceDmrCluster**
> DmrClusterResponse ReplaceDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)

Replace a Cluster object.

Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceDmrClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrCluster(); // DmrCluster | The Cluster object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Cluster object.
                DmrClusterResponse result = apiInstance.ReplaceDmrCluster(body, dmrClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.ReplaceDmrCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrCluster**](DmrCluster.md)| The Cluster object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterResponse**](DmrClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacedmrclustercertmatchingrule"></a>
# **ReplaceDmrClusterCertMatchingRule**
> DmrClusterCertMatchingRuleResponse ReplaceDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)

Replace a Certificate Matching Rule object.

Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceDmrClusterCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRule(); // DmrClusterCertMatchingRule | The Certificate Matching Rule object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Certificate Matching Rule object.
                DmrClusterCertMatchingRuleResponse result = apiInstance.ReplaceDmrClusterCertMatchingRule(body, dmrClusterName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.ReplaceDmrClusterCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRule**](DmrClusterCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleResponse**](DmrClusterCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacedmrclustercertmatchingruleattributefilter"></a>
# **ReplaceDmrClusterCertMatchingRuleAttributeFilter**
> DmrClusterCertMatchingRuleAttributeFilterResponse ReplaceDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Replace a Certificate Matching Rule Attribute Filter object.

Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceDmrClusterCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRuleAttributeFilter(); // DmrClusterCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Certificate Matching Rule Attribute Filter object.
                DmrClusterCertMatchingRuleAttributeFilterResponse result = apiInstance.ReplaceDmrClusterCertMatchingRuleAttributeFilter(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.ReplaceDmrClusterCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRuleAttributeFilter**](DmrClusterCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleAttributeFilterResponse**](DmrClusterCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacedmrclusterlink"></a>
# **ReplaceDmrClusterLink**
> DmrClusterLinkResponse ReplaceDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Replace a Link object.

Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceDmrClusterLinkExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLink(); // DmrClusterLink | The Link object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Link object.
                DmrClusterLinkResponse result = apiInstance.ReplaceDmrClusterLink(body, dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.ReplaceDmrClusterLink: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLink**](DmrClusterLink.md)| The Link object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkResponse**](DmrClusterLinkResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatedmrcluster"></a>
# **UpdateDmrCluster**
> DmrClusterResponse UpdateDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)

Update a Cluster object.

Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateDmrClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrCluster(); // DmrCluster | The Cluster object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Cluster object.
                DmrClusterResponse result = apiInstance.UpdateDmrCluster(body, dmrClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.UpdateDmrCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrCluster**](DmrCluster.md)| The Cluster object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterResponse**](DmrClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatedmrclustercertmatchingrule"></a>
# **UpdateDmrClusterCertMatchingRule**
> DmrClusterCertMatchingRuleResponse UpdateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)

Update a Certificate Matching Rule object.

Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateDmrClusterCertMatchingRuleExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRule(); // DmrClusterCertMatchingRule | The Certificate Matching Rule object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Certificate Matching Rule object.
                DmrClusterCertMatchingRuleResponse result = apiInstance.UpdateDmrClusterCertMatchingRule(body, dmrClusterName, ruleName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.UpdateDmrClusterCertMatchingRule: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRule**](DmrClusterCertMatchingRule.md)| The Certificate Matching Rule object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleResponse**](DmrClusterCertMatchingRuleResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatedmrclustercertmatchingruleattributefilter"></a>
# **UpdateDmrClusterCertMatchingRuleAttributeFilter**
> DmrClusterCertMatchingRuleAttributeFilterResponse UpdateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)

Update a Certificate Matching Rule Attribute Filter object.

Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.28.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateDmrClusterCertMatchingRuleAttributeFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterCertMatchingRuleAttributeFilter(); // DmrClusterCertMatchingRuleAttributeFilter | The Certificate Matching Rule Attribute Filter object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var ruleName = ruleName_example;  // string | The name of the rule.
            var filterName = filterName_example;  // string | The name of the filter.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Certificate Matching Rule Attribute Filter object.
                DmrClusterCertMatchingRuleAttributeFilterResponse result = apiInstance.UpdateDmrClusterCertMatchingRuleAttributeFilter(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.UpdateDmrClusterCertMatchingRuleAttributeFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterCertMatchingRuleAttributeFilter**](DmrClusterCertMatchingRuleAttributeFilter.md)| The Certificate Matching Rule Attribute Filter object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **ruleName** | **string**| The name of the rule. | 
 **filterName** | **string**| The name of the filter. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterCertMatchingRuleAttributeFilterResponse**](DmrClusterCertMatchingRuleAttributeFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatedmrclusterlink"></a>
# **UpdateDmrClusterLink**
> DmrClusterLinkResponse UpdateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)

Update a Link object.

Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateDmrClusterLinkExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DmrClusterApi();
            var body = new DmrClusterLink(); // DmrClusterLink | The Link object's attributes.
            var dmrClusterName = dmrClusterName_example;  // string | The name of the Cluster.
            var remoteNodeName = remoteNodeName_example;  // string | The name of the node at the remote end of the Link.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Link object.
                DmrClusterLinkResponse result = apiInstance.UpdateDmrClusterLink(body, dmrClusterName, remoteNodeName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DmrClusterApi.UpdateDmrClusterLink: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DmrClusterLink**](DmrClusterLink.md)| The Link object&#x27;s attributes. | 
 **dmrClusterName** | **string**| The name of the Cluster. | 
 **remoteNodeName** | **string**| The name of the node at the remote end of the Link. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DmrClusterLinkResponse**](DmrClusterLinkResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
