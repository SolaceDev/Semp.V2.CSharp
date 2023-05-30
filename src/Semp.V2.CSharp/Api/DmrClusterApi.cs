/* 
 * SEMP (Solace Element Management Protocol)
 *
 * SEMP (starting in `v2`, see note 1) is a RESTful API for configuring, monitoring, and administering a Solace PubSub+ broker.  SEMP uses URIs to address manageable **resources** of the Solace PubSub+ broker. Resources are individual **objects**, **collections** of objects, or (exclusively in the action API) **actions**. This document applies to the following API:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Configuration|/SEMP/v2/config|Reading and writing config state|See note 2    The following APIs are also available:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Action|/SEMP/v2/action|Performing actions|See note 2 Monitoring|/SEMP/v2/monitor|Querying operational parameters|See note 2    Resources are always nouns, with individual objects being singular and collections being plural.  Objects within a collection are identified by an `obj-id`, which follows the collection name with the form `collection-name/obj-id`.  Actions within an object are identified by an `action-id`, which follows the object name with the form `obj-id/action-id`.  Some examples:  ``` /SEMP/v2/config/msgVpns                        ; MsgVpn collection /SEMP/v2/config/msgVpns/a                      ; MsgVpn object named \"a\" /SEMP/v2/config/msgVpns/a/queues               ; Queue collection in MsgVpn \"a\" /SEMP/v2/config/msgVpns/a/queues/b             ; Queue object named \"b\" in MsgVpn \"a\" /SEMP/v2/action/msgVpns/a/queues/b/startReplay ; Action that starts a replay on Queue \"b\" in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients             ; Client collection in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients/c           ; Client object named \"c\" in MsgVpn \"a\" ```  ## Collection Resources  Collections are unordered lists of objects (unless described as otherwise), and are described by JSON arrays. Each item in the array represents an object in the same manner as the individual object would normally be represented. In the configuration API, the creation of a new object is done through its collection resource.  ## Object and Action Resources  Objects are composed of attributes, actions, collections, and other objects. They are described by JSON objects as name/value pairs. The collections and actions of an object are not contained directly in the object's JSON content; rather the content includes an attribute containing a URI which points to the collections and actions. These contained resources must be managed through this URI. At a minimum, every object has one or more identifying attributes, and its own `uri` attribute which contains the URI pointing to itself.  Actions are also composed of attributes, and are described by JSON objects as name/value pairs. Unlike objects, however, they are not members of a collection and cannot be retrieved, only performed. Actions only exist in the action API.  Attributes in an object or action may have any combination of the following properties:   Property|Meaning|Comments :- --|:- --|:- -- Identifying|Attribute is involved in unique identification of the object, and appears in its URI| Const|Attribute value can only be chosen during object creation| Required|Attribute must be provided in the request| Read-Only|Attribute can only be read, not written.|See note 3 Write-Only|Attribute can only be written, not read, unless the attribute is also opaque|See the documentation for the opaque property Requires-Disable|Attribute cannot be changed while the object (or the relevant part of the object) is administratively enabled| Auto-Disable|Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as one or more attributes will be temporarily disabled to apply the change| Deprecated|Attribute is deprecated, and will disappear in the next SEMP version| Opaque|Attribute can be set or retrieved in opaque form when the `opaquePassword` query parameter is present|See the `opaquePassword` query parameter documentation    In some requests, certain attributes may only be provided in certain combinations with other attributes:   Relationship|Meaning :- --|:- -- Requires|Attribute may only be changed by a request if a particular attribute or combination of attributes is also provided in the request Conflicts|Attribute may only be provided in a request if a particular attribute or combination of attributes is not also provided in the request    In the monitoring API, any non-identifying attribute may not be returned in a GET.  ## HTTP Methods  The following HTTP methods manipulate resources in accordance with these general principles. Note that some methods are only used in certain APIs:   Method|Resource|Meaning|Request Body|Response Body|Notes :- --|:- --|:- --|:- --|:- --|:- -- POST|Collection|Create object|Initial attribute values|Object attributes and metadata|Absent attributes are set to default. If object already exists, a 400 error is returned PUT|Object|Update object|New attribute values|Object attributes and metadata|If does not exist, the object is first created. Absent attributes are set to default, with certain exceptions (see note 4) PUT|Action|Performs action|Action arguments|Action metadata| PATCH|Object|Update object|New attribute values|Object attributes and metadata|Absent attributes are left unchanged. If the object does not exist, a 404 error is returned DELETE|Object|Delete object|Empty|Object metadata|If the object does not exist, a 404 is returned GET|Object|Get object|Empty|Object attributes and metadata|If the object does not exist, a 404 is returned GET|Collection|Get collection|Empty|Object attributes and collection metadata|If the collection is empty, then an empty collection is returned with a 200 code    ## Common Query Parameters  The following are some common query parameters that are supported by many method/URI combinations. Individual URIs may document additional parameters. Note that multiple query parameters can be used together in a single URI, separated by the ampersand character. For example:  ``` ; Request for the MsgVpns collection using two hypothetical query parameters ; \"q1\" and \"q2\" with values \"val1\" and \"val2\" respectively /SEMP/v2/config/msgVpns?q1=val1&q2=val2 ```  ### select  Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. Use this query parameter to limit the size of the returned data for each returned object, return only those fields that are desired, or exclude fields that are not desired.  The value of `select` is a comma-separated list of attribute names. If the list contains attribute names that are not prefaced by `-`, only those attributes are included in the response. If the list contains attribute names that are prefaced by `-`, those attributes are excluded from the response. If the list contains both types, then the difference of the first set of attributes and the second set of attributes is returned. If the list is empty (i.e. `select=`), it is treated the same as if no `select` was provided: all attribute are returned.  All attributes that are prefaced by `-` must follow all attributes that are not prefaced by `-`. In addition, each attribute name in the list must match at least one attribute in the object.  Names may include the `*` wildcard (zero or more characters). Nested attribute names are supported using periods (e.g. `parentName.childName`).  Some examples:  ``` ; List of all MsgVpn names /SEMP/v2/config/msgVpns?select=msgVpnName ; List of all MsgVpn and their attributes except for their names /SEMP/v2/config/msgVpns?select=-msgVpnName ; Authentication attributes of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance?select=authentication%2A ; All attributes of MsgVpn \"finance\" except for authentication attributes /SEMP/v2/config/msgVpns/finance?select=-authentication%2A ; Access related attributes of Queue \"orderQ\" of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance/queues/orderQ?select=owner,permission ```  ### where  Include in the response only objects where certain conditions are true. Use this query parameter to limit which objects are returned to those whose attribute values meet the given conditions.  The value of `where` is a comma-separated list of expressions. All expressions must be true for the object to be included in the response. Each expression takes the form:  ``` expression  = attribute-name OP value OP          = '==' | '!=' | '<' | '>' | '<=' | '>=' ```  `value` may be a number, string, `true`, or `false`, as appropriate for the type of `attribute-name`. Greater-than and less-than comparisons only work for numbers. A `*` in a string `value` is interpreted as a wildcard (zero or more characters). Some examples:  ``` ; Only enabled MsgVpns /SEMP/v2/config/msgVpns?where=enabled%3D%3Dtrue ; Only MsgVpns using basic non-LDAP authentication /SEMP/v2/config/msgVpns?where=authenticationBasicEnabled%3D%3Dtrue,authenticationBasicType%21%3Dldap ; Only MsgVpns that allow more than 100 client connections /SEMP/v2/config/msgVpns?where=maxConnectionCount%3E100 ; Only MsgVpns with msgVpnName starting with \"B\": /SEMP/v2/config/msgVpns?where=msgVpnName%3D%3DB%2A ```  ### count  Limit the count of objects in the response. This can be useful to limit the size of the response for large collections. The minimum value for `count` is `1` and the default is `10`. There is also a per-collection maximum value to limit request handling time.  `count` does not guarantee that a minimum number of objects will be returned. A page may contain fewer than `count` objects or even be empty. Additional objects may nonetheless be available for retrieval on subsequent pages. See the `cursor` query parameter documentation for more information on paging.  For example: ``` ; Up to 25 MsgVpns /SEMP/v2/config/msgVpns?count=25 ```  ### cursor  The cursor, or position, for the next page of objects. Cursors are opaque data that should not be created or interpreted by SEMP clients, and should only be used as described below.  When a request is made for a collection and there may be additional objects available for retrieval that are not included in the initial response, the response will include a `cursorQuery` field containing a cursor. The value of this field can be specified in the `cursor` query parameter of a subsequent request to retrieve the next page of objects.  Applications must continue to use the `cursorQuery` if one is provided in order to retrieve the full set of objects associated with the request, even if a page contains fewer than the requested number of objects (see the `count` query parameter documentation) or is empty.  ### opaquePassword  Attributes with the opaque property are also write-only and so cannot normally be retrieved in a GET. However, when a password is provided in the `opaquePassword` query parameter, attributes with the opaque property are retrieved in a GET in opaque form, encrypted with this password. The query parameter can also be used on a POST, PATCH, or PUT to set opaque attributes using opaque attribute values retrieved in a GET, so long as:  1. the same password that was used to retrieve the opaque attribute values is provided; and  2. the broker to which the request is being sent has the same major and minor SEMP version as the broker that produced the opaque attribute values.  The password provided in the query parameter must be a minimum of 8 characters and a maximum of 128 characters.  The query parameter can only be used in the configuration API, and only over HTTPS.  ## Authentication  When a client makes its first SEMPv2 request, it must supply a username and password using HTTP Basic authentication, or an OAuth token or tokens using HTTP Bearer authentication.  When HTTP Basic authentication is used, the broker returns a cookie containing a session key. The client can omit the username and password from subsequent requests, because the broker can use the session cookie for authentication instead. When the session expires or is deleted, the client must provide the username and password again, and the broker creates a new session.  There are a limited number of session slots available on the broker. The broker returns 529 No SEMP Session Available if it is not able to allocate a session.  If certain attributes—such as a user's password—are changed, the broker automatically deletes the affected sessions. These attributes are documented below. However, changes in external user configuration data stored on a RADIUS or LDAP server do not trigger the broker to delete the associated session(s), therefore you must do this manually, if required.  A client can retrieve its current session information using the /about/user endpoint and delete its own session using the /about/user/logout endpoint. A client with appropriate permissions can also manage all sessions using the /sessions endpoint.  Sessions are not created when authenticating with an OAuth token or tokens using HTTP Bearer authentication. If a session cookie is provided, it is ignored.  ## Help  Visit [our website](https://solace.com) to learn more about Solace.  You can also download the SEMP API specifications by clicking [here](https://solace.com/downloads/).  If you need additional support, please contact us at [support@solace.com](mailto:support@solace.com).  ## Notes  Note|Description :- --:|:- -- 1|This specification defines SEMP starting in \"v2\", and not the original SEMP \"v1\" interface. Request and response formats between \"v1\" and \"v2\" are entirely incompatible, although both protocols share a common port configuration on the Solace PubSub+ broker. They are differentiated by the initial portion of the URI path, one of either \"/SEMP/\" or \"/SEMP/v2/\" 2|This API is partially implemented. Only a subset of all objects are available. 3|Read-only attributes may appear in POST and PUT/PATCH requests. However, if a read-only attribute is not marked as identifying, it will be ignored during a PUT/PATCH. 4|On a PUT, if the SEMP user is not authorized to modify the attribute, its value is left unchanged rather than set to default. In addition, the values of write-only attributes are not set to their defaults on a PUT, except in the following two cases: there is a mutual requires relationship with another non-write-only attribute, both attributes are absent from the request, and the non-write-only attribute is not currently set to its default value; or the attribute is also opaque and the `opaquePassword` query parameter is provided in the request.  
 *
 * OpenAPI spec version: 2.35
 * Contact: support@solace.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp.Portable;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Semp.V2.CSharp.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public interface IDmrClusterApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create a Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        DmrClusterResponse CreateDmrCluster (DmrCluster body, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        ApiResponse<DmrClusterResponse> CreateDmrClusterWithHttpInfo (DmrCluster body, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        DmrClusterCertMatchingRuleResponse CreateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleResponse> CreateDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        DmrClusterCertMatchingRuleAttributeFilterResponse CreateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> CreateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionResponse</returns>
        DmrClusterCertMatchingRuleConditionResponse CreateDmrClusterCertMatchingRuleCondition (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleConditionResponse> CreateDmrClusterCertMatchingRuleConditionWithHttpInfo (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Link object.
        /// </summary>
        /// <remarks>
        /// Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        DmrClusterLinkResponse CreateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Link object.
        /// </summary>
        /// <remarks>
        /// Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        ApiResponse<DmrClusterLinkResponse> CreateDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributeResponse</returns>
        DmrClusterLinkAttributeResponse CreateDmrClusterLinkAttribute (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributeResponse</returns>
        ApiResponse<DmrClusterLinkAttributeResponse> CreateDmrClusterLinkAttributeWithHttpInfo (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressResponse</returns>
        DmrClusterLinkRemoteAddressResponse CreateDmrClusterLinkRemoteAddress (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressResponse</returns>
        ApiResponse<DmrClusterLinkRemoteAddressResponse> CreateDmrClusterLinkRemoteAddressWithHttpInfo (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        DmrClusterLinkTlsTrustedCommonNameResponse CreateDmrClusterLinkTlsTrustedCommonName (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> CreateDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrCluster (string dmrClusterName);

        /// <summary>
        /// Delete a Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterWithHttpInfo (string dmrClusterName);
        /// <summary>
        /// Delete a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterCertMatchingRule (string dmrClusterName, string ruleName);

        /// <summary>
        /// Delete a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleWithHttpInfo (string dmrClusterName, string ruleName);
        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName);

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (string dmrClusterName, string ruleName, string filterName);
        /// <summary>
        /// Delete a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source);

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleConditionWithHttpInfo (string dmrClusterName, string ruleName, string source);
        /// <summary>
        /// Delete a Link object.
        /// </summary>
        /// <remarks>
        /// Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterLink (string dmrClusterName, string remoteNodeName);

        /// <summary>
        /// Delete a Link object.
        /// </summary>
        /// <remarks>
        /// Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterLinkWithHttpInfo (string dmrClusterName, string remoteNodeName);
        /// <summary>
        /// Delete a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue);

        /// <summary>
        /// Delete a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterLinkAttributeWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue);
        /// <summary>
        /// Delete a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress);

        /// <summary>
        /// Delete a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterLinkRemoteAddressWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress);
        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName);

        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName);
        /// <summary>
        /// Get a Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        DmrClusterResponse GetDmrCluster (string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        ApiResponse<DmrClusterResponse> GetDmrClusterWithHttpInfo (string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        DmrClusterCertMatchingRuleResponse GetDmrClusterCertMatchingRule (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleResponse> GetDmrClusterCertMatchingRuleWithHttpInfo (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        DmrClusterCertMatchingRuleAttributeFilterResponse GetDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> GetDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        DmrClusterCertMatchingRuleAttributeFiltersResponse GetDmrClusterCertMatchingRuleAttributeFilters (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse> GetDmrClusterCertMatchingRuleAttributeFiltersWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionResponse</returns>
        DmrClusterCertMatchingRuleConditionResponse GetDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleConditionResponse> GetDmrClusterCertMatchingRuleConditionWithHttpInfo (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionsResponse</returns>
        DmrClusterCertMatchingRuleConditionsResponse GetDmrClusterCertMatchingRuleConditions (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionsResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleConditionsResponse> GetDmrClusterCertMatchingRuleConditionsWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRulesResponse</returns>
        DmrClusterCertMatchingRulesResponse GetDmrClusterCertMatchingRules (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRulesResponse</returns>
        ApiResponse<DmrClusterCertMatchingRulesResponse> GetDmrClusterCertMatchingRulesWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Link object.
        /// </summary>
        /// <remarks>
        /// Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        DmrClusterLinkResponse GetDmrClusterLink (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Link object.
        /// </summary>
        /// <remarks>
        /// Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        ApiResponse<DmrClusterLinkResponse> GetDmrClusterLinkWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributeResponse</returns>
        DmrClusterLinkAttributeResponse GetDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributeResponse</returns>
        ApiResponse<DmrClusterLinkAttributeResponse> GetDmrClusterLinkAttributeWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Link Attribute objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributesResponse</returns>
        DmrClusterLinkAttributesResponse GetDmrClusterLinkAttributes (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Link Attribute objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributesResponse</returns>
        ApiResponse<DmrClusterLinkAttributesResponse> GetDmrClusterLinkAttributesWithHttpInfo (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressResponse</returns>
        DmrClusterLinkRemoteAddressResponse GetDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressResponse</returns>
        ApiResponse<DmrClusterLinkRemoteAddressResponse> GetDmrClusterLinkRemoteAddressWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Address objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressesResponse</returns>
        DmrClusterLinkRemoteAddressesResponse GetDmrClusterLinkRemoteAddresses (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Address objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressesResponse</returns>
        ApiResponse<DmrClusterLinkRemoteAddressesResponse> GetDmrClusterLinkRemoteAddressesWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        DmrClusterLinkTlsTrustedCommonNameResponse GetDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> GetDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        DmrClusterLinkTlsTrustedCommonNamesResponse GetDmrClusterLinkTlsTrustedCommonNames (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse> GetDmrClusterLinkTlsTrustedCommonNamesWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Link objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinksResponse</returns>
        DmrClusterLinksResponse GetDmrClusterLinks (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Link objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinksResponse</returns>
        ApiResponse<DmrClusterLinksResponse> GetDmrClusterLinksWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClustersResponse</returns>
        DmrClustersResponse GetDmrClusters (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClustersResponse</returns>
        ApiResponse<DmrClustersResponse> GetDmrClustersWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        DmrClusterResponse ReplaceDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        ApiResponse<DmrClusterResponse> ReplaceDmrClusterWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        DmrClusterCertMatchingRuleResponse ReplaceDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleResponse> ReplaceDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        DmrClusterCertMatchingRuleAttributeFilterResponse ReplaceDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> ReplaceDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Link object.
        /// </summary>
        /// <remarks>
        /// Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        DmrClusterLinkResponse ReplaceDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Link object.
        /// </summary>
        /// <remarks>
        /// Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        ApiResponse<DmrClusterLinkResponse> ReplaceDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        DmrClusterResponse UpdateDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        ApiResponse<DmrClusterResponse> UpdateDmrClusterWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        DmrClusterCertMatchingRuleResponse UpdateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleResponse> UpdateDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        DmrClusterCertMatchingRuleAttributeFilterResponse UpdateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> UpdateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Link object.
        /// </summary>
        /// <remarks>
        /// Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        DmrClusterLinkResponse UpdateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Link object.
        /// </summary>
        /// <remarks>
        /// Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        ApiResponse<DmrClusterLinkResponse> UpdateDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create a Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterResponse> CreateDmrClusterAsync (DmrCluster body, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> CreateDmrClusterAsyncWithHttpInfo (DmrCluster body, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> CreateDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> CreateDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> CreateDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> CreateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionResponse> CreateDmrClusterCertMatchingRuleConditionAsync (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionResponse>> CreateDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Link object.
        /// </summary>
        /// <remarks>
        /// Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkResponse> CreateDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Link object.
        /// </summary>
        /// <remarks>
        /// Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> CreateDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributeResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkAttributeResponse> CreateDmrClusterLinkAttributeAsync (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributeResponse>> CreateDmrClusterLinkAttributeAsyncWithHttpInfo (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressResponse> CreateDmrClusterLinkRemoteAddressAsync (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressResponse>> CreateDmrClusterLinkRemoteAddressAsyncWithHttpInfo (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNameResponse> CreateDmrClusterLinkTlsTrustedCommonNameAsync (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNameResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>> CreateDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterAsync (string dmrClusterName);

        /// <summary>
        /// Delete a Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterAsyncWithHttpInfo (string dmrClusterName);
        /// <summary>
        /// Delete a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleAsync (string dmrClusterName, string ruleName);

        /// <summary>
        /// Delete a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleAsyncWithHttpInfo (string dmrClusterName, string ruleName);
        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleAttributeFilterAsync (string dmrClusterName, string ruleName, string filterName);

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (string dmrClusterName, string ruleName, string filterName);
        /// <summary>
        /// Delete a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleConditionAsync (string dmrClusterName, string ruleName, string source);

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (string dmrClusterName, string ruleName, string source);
        /// <summary>
        /// Delete a Link object.
        /// </summary>
        /// <remarks>
        /// Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkAsync (string dmrClusterName, string remoteNodeName);

        /// <summary>
        /// Delete a Link object.
        /// </summary>
        /// <remarks>
        /// Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName);
        /// <summary>
        /// Delete a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkAttributeAsync (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue);

        /// <summary>
        /// Delete a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkAttributeAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue);
        /// <summary>
        /// Delete a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkRemoteAddressAsync (string dmrClusterName, string remoteNodeName, string remoteAddress);

        /// <summary>
        /// Delete a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkRemoteAddressAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress);
        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkTlsTrustedCommonNameAsync (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName);

        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName);
        /// <summary>
        /// Get a Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterResponse> GetDmrClusterAsync (string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> GetDmrClusterAsyncWithHttpInfo (string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> GetDmrClusterCertMatchingRuleAsync (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> GetDmrClusterCertMatchingRuleAsyncWithHttpInfo (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> GetDmrClusterCertMatchingRuleAttributeFilterAsync (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> GetDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFiltersResponse> GetDmrClusterCertMatchingRuleAttributeFiltersAsync (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFiltersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse>> GetDmrClusterCertMatchingRuleAttributeFiltersAsyncWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionResponse> GetDmrClusterCertMatchingRuleConditionAsync (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Certificate Matching Rule Condition object.
        /// </summary>
        /// <remarks>
        /// Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionResponse>> GetDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionsResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionsResponse> GetDmrClusterCertMatchingRuleConditionsAsync (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionsResponse>> GetDmrClusterCertMatchingRuleConditionsAsyncWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Certificate Matching Rule objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRulesResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRulesResponse> GetDmrClusterCertMatchingRulesAsync (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Certificate Matching Rule objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRulesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRulesResponse>> GetDmrClusterCertMatchingRulesAsyncWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Link object.
        /// </summary>
        /// <remarks>
        /// Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkResponse> GetDmrClusterLinkAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Link object.
        /// </summary>
        /// <remarks>
        /// Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> GetDmrClusterLinkAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributeResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkAttributeResponse> GetDmrClusterLinkAttributeAsync (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Link Attribute object.
        /// </summary>
        /// <remarks>
        /// Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributeResponse>> GetDmrClusterLinkAttributeAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Link Attribute objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributesResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkAttributesResponse> GetDmrClusterLinkAttributesAsync (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Link Attribute objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributesResponse>> GetDmrClusterLinkAttributesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressResponse> GetDmrClusterLinkRemoteAddressAsync (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Address object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressResponse>> GetDmrClusterLinkRemoteAddressAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Address objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressesResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressesResponse> GetDmrClusterLinkRemoteAddressesAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Address objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressesResponse>> GetDmrClusterLinkRemoteAddressesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNameResponse> GetDmrClusterLinkTlsTrustedCommonNameAsync (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNameResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>> GetDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNamesResponse> GetDmrClusterLinkTlsTrustedCommonNamesAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNamesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse>> GetDmrClusterLinkTlsTrustedCommonNamesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Link objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinksResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinksResponse> GetDmrClusterLinksAsync (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Link objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinksResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinksResponse>> GetDmrClusterLinksAsyncWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClustersResponse</returns>
        System.Threading.Tasks.Task<DmrClustersResponse> GetDmrClustersAsync (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClustersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClustersResponse>> GetDmrClustersAsyncWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterResponse> ReplaceDmrClusterAsync (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> ReplaceDmrClusterAsyncWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> ReplaceDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> ReplaceDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> ReplaceDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> ReplaceDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Link object.
        /// </summary>
        /// <remarks>
        /// Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkResponse> ReplaceDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Link object.
        /// </summary>
        /// <remarks>
        /// Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> ReplaceDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterResponse> UpdateDmrClusterAsync (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> UpdateDmrClusterAsyncWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> UpdateDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Certificate Matching Rule object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> UpdateDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> UpdateDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> UpdateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Link object.
        /// </summary>
        /// <remarks>
        /// Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        System.Threading.Tasks.Task<DmrClusterLinkResponse> UpdateDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Link object.
        /// </summary>
        /// <remarks>
        /// Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> UpdateDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class DmrClusterApi : IDmrClusterApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmrClusterApi"/> class.
        /// </summary>
        /// <returns></returns>
        public DmrClusterApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmrClusterApi"/> class
        /// </summary>
        /// <returns></returns>
        public DmrClusterApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmrClusterApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public DmrClusterApi(Semp.V2.CSharp.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Semp.V2.CSharp.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Semp.V2.CSharp.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Create a Cluster object. Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        public DmrClusterResponse CreateDmrCluster (DmrCluster body, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = CreateDmrClusterWithHttpInfo(body, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Cluster object. Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        public ApiResponse< DmrClusterResponse > CreateDmrClusterWithHttpInfo (DmrCluster body, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrCluster");

            var localVarPath = "./dmrClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Create a Cluster object. Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterResponse> CreateDmrClusterAsync (DmrCluster body, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = await CreateDmrClusterAsyncWithHttpInfo(body, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Cluster object. Create a Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| dmrClusterName|x|x|||| nodeName|||x||| tlsServerCertEnforceTrustedCommonNameEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> CreateDmrClusterAsyncWithHttpInfo (DmrCluster body, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrCluster");

            var localVarPath = "./dmrClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule object. Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        public DmrClusterCertMatchingRuleResponse CreateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = CreateDmrClusterCertMatchingRuleWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Certificate Matching Rule object. Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleResponse > CreateDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule object. Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> CreateDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = await CreateDmrClusterCertMatchingRuleAsyncWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Certificate Matching Rule object. Create a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> CreateDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object. Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public DmrClusterCertMatchingRuleAttributeFilterResponse CreateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = CreateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object. Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleAttributeFilterResponse > CreateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object. Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> CreateDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = await CreateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Certificate Matching Rule Attribute Filter object. Create a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| filterName|x|x|||| ruleName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> CreateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule Condition object. Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionResponse</returns>
        public DmrClusterCertMatchingRuleConditionResponse CreateDmrClusterCertMatchingRuleCondition (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionResponse> localVarResponse = CreateDmrClusterCertMatchingRuleConditionWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Certificate Matching Rule Condition object. Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleConditionResponse > CreateDmrClusterCertMatchingRuleConditionWithHttpInfo (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionResponse)));
        }

        /// <summary>
        /// Create a Certificate Matching Rule Condition object. Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionResponse> CreateDmrClusterCertMatchingRuleConditionAsync (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionResponse> localVarResponse = await CreateDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Certificate Matching Rule Condition object. Create a Certificate Matching Rule Condition object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| ruleName|x||x||| source|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Condition object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionResponse>> CreateDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (DmrClusterCertMatchingRuleCondition body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->CreateDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionResponse)));
        }

        /// <summary>
        /// Create a Link object. Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        public DmrClusterLinkResponse CreateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = CreateDmrClusterLinkWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Link object. Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        public ApiResponse< DmrClusterLinkResponse > CreateDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Create a Link object. Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkResponse> CreateDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = await CreateDmrClusterLinkAsyncWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Link object. Create a Link object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||||x||x dmrClusterName|x||x||| remoteNodeName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> CreateDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Create a Link Attribute object. Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributeResponse</returns>
        public DmrClusterLinkAttributeResponse CreateDmrClusterLinkAttribute (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributeResponse> localVarResponse = CreateDmrClusterLinkAttributeWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Link Attribute object. Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributeResponse</returns>
        public ApiResponse< DmrClusterLinkAttributeResponse > CreateDmrClusterLinkAttributeWithHttpInfo (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributeResponse)));
        }

        /// <summary>
        /// Create a Link Attribute object. Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributeResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkAttributeResponse> CreateDmrClusterLinkAttributeAsync (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributeResponse> localVarResponse = await CreateDmrClusterLinkAttributeAsyncWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Link Attribute object. Create a Link Attribute object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: attributeName|x|x|||| attributeValue|x|x|||| dmrClusterName|x||x||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link Attribute object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributeResponse>> CreateDmrClusterLinkAttributeAsyncWithHttpInfo (DmrClusterLinkAttribute body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributeResponse)));
        }

        /// <summary>
        /// Create a Remote Address object. Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressResponse</returns>
        public DmrClusterLinkRemoteAddressResponse CreateDmrClusterLinkRemoteAddress (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressResponse> localVarResponse = CreateDmrClusterLinkRemoteAddressWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Remote Address object. Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressResponse</returns>
        public ApiResponse< DmrClusterLinkRemoteAddressResponse > CreateDmrClusterLinkRemoteAddressWithHttpInfo (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressResponse)));
        }

        /// <summary>
        /// Create a Remote Address object. Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressResponse> CreateDmrClusterLinkRemoteAddressAsync (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressResponse> localVarResponse = await CreateDmrClusterLinkRemoteAddressAsyncWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Remote Address object. Create a Remote Address object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||| remoteAddress|x|x|||| remoteNodeName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Address object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressResponse>> CreateDmrClusterLinkRemoteAddressAsyncWithHttpInfo (DmrClusterLinkRemoteAddress body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressResponse)));
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public DmrClusterLinkTlsTrustedCommonNameResponse CreateDmrClusterLinkTlsTrustedCommonName (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> localVarResponse = CreateDmrClusterLinkTlsTrustedCommonNameWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public ApiResponse< DmrClusterLinkTlsTrustedCommonNameResponse > CreateDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNameResponse> CreateDmrClusterLinkTlsTrustedCommonNameAsync (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> localVarResponse = await CreateDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x||x| remoteNodeName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNameResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>> CreateDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (DmrClusterLinkTlsTrustedCommonName body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->CreateDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Delete a Cluster object. Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrCluster (string dmrClusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterWithHttpInfo(dmrClusterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Cluster object. Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterWithHttpInfo (string dmrClusterName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Cluster object. Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterAsync (string dmrClusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterAsyncWithHttpInfo(dmrClusterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Cluster object. Delete a Cluster object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterAsyncWithHttpInfo (string dmrClusterName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule object. Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterCertMatchingRule (string dmrClusterName, string ruleName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterCertMatchingRuleWithHttpInfo(dmrClusterName, ruleName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Certificate Matching Rule object. Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterCertMatchingRuleWithHttpInfo (string dmrClusterName, string ruleName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule object. Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleAsync (string dmrClusterName, string ruleName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterCertMatchingRuleAsyncWithHttpInfo(dmrClusterName, ruleName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Certificate Matching Rule object. Delete a Certificate Matching Rule object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleAsyncWithHttpInfo (string dmrClusterName, string ruleName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object. Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo(dmrClusterName, ruleName, filterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object. Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (string dmrClusterName, string ruleName, string filterName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object. Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleAttributeFilterAsync (string dmrClusterName, string ruleName, string filterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo(dmrClusterName, ruleName, filterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Certificate Matching Rule Attribute Filter object. Delete a Certificate Matching Rule Attribute Filter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (string dmrClusterName, string ruleName, string filterName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object. Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterCertMatchingRuleConditionWithHttpInfo(dmrClusterName, ruleName, source);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object. Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterCertMatchingRuleConditionWithHttpInfo (string dmrClusterName, string ruleName, string source)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'source' is set
            if (source == null)
                throw new ApiException(400, "Missing required parameter 'source' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (source != null) localVarPathParams.Add("source", this.Configuration.ApiClient.ParameterToString(source)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object. Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterCertMatchingRuleConditionAsync (string dmrClusterName, string ruleName, string source)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo(dmrClusterName, ruleName, source);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Certificate Matching Rule Condition object. Delete a Certificate Matching Rule Condition object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (string dmrClusterName, string ruleName, string source)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'source' is set
            if (source == null)
                throw new ApiException(400, "Missing required parameter 'source' when calling DmrClusterApi->DeleteDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (source != null) localVarPathParams.Add("source", this.Configuration.ApiClient.ParameterToString(source)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Link object. Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterLink (string dmrClusterName, string remoteNodeName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterLinkWithHttpInfo(dmrClusterName, remoteNodeName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Link object. Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterLinkWithHttpInfo (string dmrClusterName, string remoteNodeName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Link object. Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkAsync (string dmrClusterName, string remoteNodeName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterLinkAsyncWithHttpInfo(dmrClusterName, remoteNodeName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Link object. Delete a Link object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Link Attribute object. Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterLinkAttributeWithHttpInfo(dmrClusterName, remoteNodeName, attributeName, attributeValue);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Link Attribute object. Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterLinkAttributeWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'attributeName' is set
            if (attributeName == null)
                throw new ApiException(400, "Missing required parameter 'attributeName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'attributeValue' is set
            if (attributeValue == null)
                throw new ApiException(400, "Missing required parameter 'attributeValue' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (attributeName != null) localVarPathParams.Add("attributeName", this.Configuration.ApiClient.ParameterToString(attributeName)); // path parameter
            if (attributeValue != null) localVarPathParams.Add("attributeValue", this.Configuration.ApiClient.ParameterToString(attributeValue)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Link Attribute object. Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkAttributeAsync (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterLinkAttributeAsyncWithHttpInfo(dmrClusterName, remoteNodeName, attributeName, attributeValue);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Link Attribute object. Delete a Link Attribute object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkAttributeAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'attributeName' is set
            if (attributeName == null)
                throw new ApiException(400, "Missing required parameter 'attributeName' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");
            // verify the required parameter 'attributeValue' is set
            if (attributeValue == null)
                throw new ApiException(400, "Missing required parameter 'attributeValue' when calling DmrClusterApi->DeleteDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (attributeName != null) localVarPathParams.Add("attributeName", this.Configuration.ApiClient.ParameterToString(attributeName)); // path parameter
            if (attributeValue != null) localVarPathParams.Add("attributeValue", this.Configuration.ApiClient.ParameterToString(attributeValue)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Address object. Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterLinkRemoteAddressWithHttpInfo(dmrClusterName, remoteNodeName, remoteAddress);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Remote Address object. Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterLinkRemoteAddressWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteAddress' is set
            if (remoteAddress == null)
                throw new ApiException(400, "Missing required parameter 'remoteAddress' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (remoteAddress != null) localVarPathParams.Add("remoteAddress", this.Configuration.ApiClient.ParameterToString(remoteAddress)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Address object. Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkRemoteAddressAsync (string dmrClusterName, string remoteNodeName, string remoteAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterLinkRemoteAddressAsyncWithHttpInfo(dmrClusterName, remoteNodeName, remoteAddress);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Remote Address object. Delete a Remote Address object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkRemoteAddressAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteAddress' is set
            if (remoteAddress == null)
                throw new ApiException(400, "Missing required parameter 'remoteAddress' when calling DmrClusterApi->DeleteDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (remoteAddress != null) localVarPathParams.Add("remoteAddress", this.Configuration.ApiClient.ParameterToString(remoteAddress)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteDmrClusterLinkTlsTrustedCommonNameWithHttpInfo(dmrClusterName, remoteNodeName, tlsTrustedCommonName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (tlsTrustedCommonName != null) localVarPathParams.Add("tlsTrustedCommonName", this.Configuration.ApiClient.ParameterToString(tlsTrustedCommonName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteDmrClusterLinkTlsTrustedCommonNameAsync (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo(dmrClusterName, remoteNodeName, tlsTrustedCommonName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling DmrClusterApi->DeleteDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (tlsTrustedCommonName != null) localVarPathParams.Add("tlsTrustedCommonName", this.Configuration.ApiClient.ParameterToString(tlsTrustedCommonName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get a Cluster object. Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        public DmrClusterResponse GetDmrCluster (string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = GetDmrClusterWithHttpInfo(dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Cluster object. Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        public ApiResponse< DmrClusterResponse > GetDmrClusterWithHttpInfo (string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Get a Cluster object. Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterResponse> GetDmrClusterAsync (string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = await GetDmrClusterAsyncWithHttpInfo(dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Cluster object. Get a Cluster object.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> GetDmrClusterAsyncWithHttpInfo (string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule object. Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        public DmrClusterCertMatchingRuleResponse GetDmrClusterCertMatchingRule (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = GetDmrClusterCertMatchingRuleWithHttpInfo(dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Certificate Matching Rule object. Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleResponse > GetDmrClusterCertMatchingRuleWithHttpInfo (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule object. Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> GetDmrClusterCertMatchingRuleAsync (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = await GetDmrClusterCertMatchingRuleAsyncWithHttpInfo(dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Certificate Matching Rule object. Get a Certificate Matching Rule object.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> GetDmrClusterCertMatchingRuleAsyncWithHttpInfo (string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object. Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public DmrClusterCertMatchingRuleAttributeFilterResponse GetDmrClusterCertMatchingRuleAttributeFilter (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = GetDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo(dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object. Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleAttributeFilterResponse > GetDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object. Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> GetDmrClusterCertMatchingRuleAttributeFilterAsync (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = await GetDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo(dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Certificate Matching Rule Attribute Filter object. Get a Certificate Matching Rule Attribute Filter object.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> GetDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects. Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        public DmrClusterCertMatchingRuleAttributeFiltersResponse GetDmrClusterCertMatchingRuleAttributeFilters (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse> localVarResponse = GetDmrClusterCertMatchingRuleAttributeFiltersWithHttpInfo(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects. Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleAttributeFiltersResponse > GetDmrClusterCertMatchingRuleAttributeFiltersWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilters");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilters");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleAttributeFilters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFiltersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFiltersResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects. Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFiltersResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFiltersResponse> GetDmrClusterCertMatchingRuleAttributeFiltersAsync (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse> localVarResponse = await GetDmrClusterCertMatchingRuleAttributeFiltersAsyncWithHttpInfo(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Attribute Filter objects. Get a list of Certificate Matching Rule Attribute Filter objects.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| filterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFiltersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse>> GetDmrClusterCertMatchingRuleAttributeFiltersAsyncWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilters");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleAttributeFilters");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleAttributeFilters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFiltersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFiltersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFiltersResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule Condition object. Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionResponse</returns>
        public DmrClusterCertMatchingRuleConditionResponse GetDmrClusterCertMatchingRuleCondition (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionResponse> localVarResponse = GetDmrClusterCertMatchingRuleConditionWithHttpInfo(dmrClusterName, ruleName, source, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Certificate Matching Rule Condition object. Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleConditionResponse > GetDmrClusterCertMatchingRuleConditionWithHttpInfo (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'source' is set
            if (source == null)
                throw new ApiException(400, "Missing required parameter 'source' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (source != null) localVarPathParams.Add("source", this.Configuration.ApiClient.ParameterToString(source)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionResponse)));
        }

        /// <summary>
        /// Get a Certificate Matching Rule Condition object. Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionResponse> GetDmrClusterCertMatchingRuleConditionAsync (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionResponse> localVarResponse = await GetDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo(dmrClusterName, ruleName, source, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Certificate Matching Rule Condition object. Get a Certificate Matching Rule Condition object.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="source">Certificate field to be compared with the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionResponse>> GetDmrClusterCertMatchingRuleConditionAsyncWithHttpInfo (string dmrClusterName, string ruleName, string source, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");
            // verify the required parameter 'source' is set
            if (source == null)
                throw new ApiException(400, "Missing required parameter 'source' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleCondition");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (source != null) localVarPathParams.Add("source", this.Configuration.ApiClient.ParameterToString(source)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleCondition", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects. Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleConditionsResponse</returns>
        public DmrClusterCertMatchingRuleConditionsResponse GetDmrClusterCertMatchingRuleConditions (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionsResponse> localVarResponse = GetDmrClusterCertMatchingRuleConditionsWithHttpInfo(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects. Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleConditionsResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleConditionsResponse > GetDmrClusterCertMatchingRuleConditionsWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleConditions");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleConditions");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleConditions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionsResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects. Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleConditionsResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleConditionsResponse> GetDmrClusterCertMatchingRuleConditionsAsync (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleConditionsResponse> localVarResponse = await GetDmrClusterCertMatchingRuleConditionsAsyncWithHttpInfo(dmrClusterName, ruleName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Certificate Matching Rule Condition objects. Get a list of Certificate Matching Rule Condition objects.  A Cert Matching Rule Condition compares data extracted from a certificate to a link attribute or an expression.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x||| source|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleConditionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleConditionsResponse>> GetDmrClusterCertMatchingRuleConditionsAsyncWithHttpInfo (string dmrClusterName, string ruleName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleConditions");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->GetDmrClusterCertMatchingRuleConditions");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRuleConditions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleConditionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleConditionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleConditionsResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule objects. Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRulesResponse</returns>
        public DmrClusterCertMatchingRulesResponse GetDmrClusterCertMatchingRules (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRulesResponse> localVarResponse = GetDmrClusterCertMatchingRulesWithHttpInfo(dmrClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule objects. Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRulesResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRulesResponse > GetDmrClusterCertMatchingRulesWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRules");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRules", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRulesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRulesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRulesResponse)));
        }

        /// <summary>
        /// Get a list of Certificate Matching Rule objects. Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRulesResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRulesResponse> GetDmrClusterCertMatchingRulesAsync (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRulesResponse> localVarResponse = await GetDmrClusterCertMatchingRulesAsyncWithHttpInfo(dmrClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Certificate Matching Rule objects. Get a list of Certificate Matching Rule objects.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| ruleName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRulesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRulesResponse>> GetDmrClusterCertMatchingRulesAsyncWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterCertMatchingRules");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterCertMatchingRules", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRulesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRulesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRulesResponse)));
        }

        /// <summary>
        /// Get a Link object. Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        public DmrClusterLinkResponse GetDmrClusterLink (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = GetDmrClusterLinkWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Link object. Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        public ApiResponse< DmrClusterLinkResponse > GetDmrClusterLinkWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Get a Link object. Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkResponse> GetDmrClusterLinkAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = await GetDmrClusterLinkAsyncWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Link object. Get a Link object.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> GetDmrClusterLinkAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Get a Link Attribute object. Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributeResponse</returns>
        public DmrClusterLinkAttributeResponse GetDmrClusterLinkAttribute (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributeResponse> localVarResponse = GetDmrClusterLinkAttributeWithHttpInfo(dmrClusterName, remoteNodeName, attributeName, attributeValue, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Link Attribute object. Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributeResponse</returns>
        public ApiResponse< DmrClusterLinkAttributeResponse > GetDmrClusterLinkAttributeWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'attributeName' is set
            if (attributeName == null)
                throw new ApiException(400, "Missing required parameter 'attributeName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'attributeValue' is set
            if (attributeValue == null)
                throw new ApiException(400, "Missing required parameter 'attributeValue' when calling DmrClusterApi->GetDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (attributeName != null) localVarPathParams.Add("attributeName", this.Configuration.ApiClient.ParameterToString(attributeName)); // path parameter
            if (attributeValue != null) localVarPathParams.Add("attributeValue", this.Configuration.ApiClient.ParameterToString(attributeValue)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributeResponse)));
        }

        /// <summary>
        /// Get a Link Attribute object. Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributeResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkAttributeResponse> GetDmrClusterLinkAttributeAsync (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributeResponse> localVarResponse = await GetDmrClusterLinkAttributeAsyncWithHttpInfo(dmrClusterName, remoteNodeName, attributeName, attributeValue, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Link Attribute object. Get a Link Attribute object.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="attributeName">The name of the Attribute.</param>
        /// <param name="attributeValue">The value of the Attribute.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributeResponse>> GetDmrClusterLinkAttributeAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string attributeName, string attributeValue, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'attributeName' is set
            if (attributeName == null)
                throw new ApiException(400, "Missing required parameter 'attributeName' when calling DmrClusterApi->GetDmrClusterLinkAttribute");
            // verify the required parameter 'attributeValue' is set
            if (attributeValue == null)
                throw new ApiException(400, "Missing required parameter 'attributeValue' when calling DmrClusterApi->GetDmrClusterLinkAttribute");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (attributeName != null) localVarPathParams.Add("attributeName", this.Configuration.ApiClient.ParameterToString(attributeName)); // path parameter
            if (attributeValue != null) localVarPathParams.Add("attributeValue", this.Configuration.ApiClient.ParameterToString(attributeValue)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkAttribute", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributeResponse)));
        }

        /// <summary>
        /// Get a list of Link Attribute objects. Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkAttributesResponse</returns>
        public DmrClusterLinkAttributesResponse GetDmrClusterLinkAttributes (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributesResponse> localVarResponse = GetDmrClusterLinkAttributesWithHttpInfo(dmrClusterName, remoteNodeName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Link Attribute objects. Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkAttributesResponse</returns>
        public ApiResponse< DmrClusterLinkAttributesResponse > GetDmrClusterLinkAttributesWithHttpInfo (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkAttributes");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkAttributes");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkAttributes", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributesResponse)));
        }

        /// <summary>
        /// Get a list of Link Attribute objects. Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkAttributesResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkAttributesResponse> GetDmrClusterLinkAttributesAsync (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkAttributesResponse> localVarResponse = await GetDmrClusterLinkAttributesAsyncWithHttpInfo(dmrClusterName, remoteNodeName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Link Attribute objects. Get a list of Link Attribute objects.  A Link Attribute is a key+value pair that can be used to locate a DMR Cluster Link, for example when using client certificate mapping.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: attributeName|x||| attributeValue|x||| dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkAttributesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkAttributesResponse>> GetDmrClusterLinkAttributesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkAttributes");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkAttributes");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkAttributes", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkAttributesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkAttributesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkAttributesResponse)));
        }

        /// <summary>
        /// Get a Remote Address object. Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressResponse</returns>
        public DmrClusterLinkRemoteAddressResponse GetDmrClusterLinkRemoteAddress (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressResponse> localVarResponse = GetDmrClusterLinkRemoteAddressWithHttpInfo(dmrClusterName, remoteNodeName, remoteAddress, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Remote Address object. Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressResponse</returns>
        public ApiResponse< DmrClusterLinkRemoteAddressResponse > GetDmrClusterLinkRemoteAddressWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteAddress' is set
            if (remoteAddress == null)
                throw new ApiException(400, "Missing required parameter 'remoteAddress' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (remoteAddress != null) localVarPathParams.Add("remoteAddress", this.Configuration.ApiClient.ParameterToString(remoteAddress)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressResponse)));
        }

        /// <summary>
        /// Get a Remote Address object. Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressResponse> GetDmrClusterLinkRemoteAddressAsync (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressResponse> localVarResponse = await GetDmrClusterLinkRemoteAddressAsyncWithHttpInfo(dmrClusterName, remoteNodeName, remoteAddress, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Remote Address object. Get a Remote Address object.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="remoteAddress">The FQDN or IP address (and optional port) of the remote node. If a port is not provided, it will vary based on the transport encoding: 55555 (plain-text), 55443 (encrypted), or 55003 (compressed).</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressResponse>> GetDmrClusterLinkRemoteAddressAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string remoteAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");
            // verify the required parameter 'remoteAddress' is set
            if (remoteAddress == null)
                throw new ApiException(400, "Missing required parameter 'remoteAddress' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddress");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (remoteAddress != null) localVarPathParams.Add("remoteAddress", this.Configuration.ApiClient.ParameterToString(remoteAddress)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkRemoteAddress", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressResponse)));
        }

        /// <summary>
        /// Get a list of Remote Address objects. Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkRemoteAddressesResponse</returns>
        public DmrClusterLinkRemoteAddressesResponse GetDmrClusterLinkRemoteAddresses (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressesResponse> localVarResponse = GetDmrClusterLinkRemoteAddressesWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Remote Address objects. Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkRemoteAddressesResponse</returns>
        public ApiResponse< DmrClusterLinkRemoteAddressesResponse > GetDmrClusterLinkRemoteAddressesWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddresses");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddresses");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkRemoteAddresses", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressesResponse)));
        }

        /// <summary>
        /// Get a list of Remote Address objects. Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkRemoteAddressesResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkRemoteAddressesResponse> GetDmrClusterLinkRemoteAddressesAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkRemoteAddressesResponse> localVarResponse = await GetDmrClusterLinkRemoteAddressesAsyncWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Remote Address objects. Get a list of Remote Address objects.  Each Remote Address, consisting of a FQDN or IP address and optional port, is used to connect to the remote node for this Link. Up to 4 addresses may be provided for each Link, and will be tried on a round-robin basis.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||| remoteAddress|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkRemoteAddressesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkRemoteAddressesResponse>> GetDmrClusterLinkRemoteAddressesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddresses");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkRemoteAddresses");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkRemoteAddresses", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkRemoteAddressesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkRemoteAddressesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkRemoteAddressesResponse)));
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public DmrClusterLinkTlsTrustedCommonNameResponse GetDmrClusterLinkTlsTrustedCommonName (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> localVarResponse = GetDmrClusterLinkTlsTrustedCommonNameWithHttpInfo(dmrClusterName, remoteNodeName, tlsTrustedCommonName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public ApiResponse< DmrClusterLinkTlsTrustedCommonNameResponse > GetDmrClusterLinkTlsTrustedCommonNameWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (tlsTrustedCommonName != null) localVarPathParams.Add("tlsTrustedCommonName", this.Configuration.ApiClient.ParameterToString(tlsTrustedCommonName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNameResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNameResponse> GetDmrClusterLinkTlsTrustedCommonNameAsync (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse> localVarResponse = await GetDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo(dmrClusterName, remoteNodeName, tlsTrustedCommonName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNameResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>> GetDmrClusterLinkTlsTrustedCommonNameAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonName");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (tlsTrustedCommonName != null) localVarPathParams.Add("tlsTrustedCommonName", this.Configuration.ApiClient.ParameterToString(tlsTrustedCommonName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        public DmrClusterLinkTlsTrustedCommonNamesResponse GetDmrClusterLinkTlsTrustedCommonNames (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse> localVarResponse = GetDmrClusterLinkTlsTrustedCommonNamesWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        public ApiResponse< DmrClusterLinkTlsTrustedCommonNamesResponse > GetDmrClusterLinkTlsTrustedCommonNamesWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonNames");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonNames");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkTlsTrustedCommonNames", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNamesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNamesResponse)));
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkTlsTrustedCommonNamesResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkTlsTrustedCommonNamesResponse> GetDmrClusterLinkTlsTrustedCommonNamesAsync (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse> localVarResponse = await GetDmrClusterLinkTlsTrustedCommonNamesAsyncWithHttpInfo(dmrClusterName, remoteNodeName, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Link are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: dmrClusterName|x||x| remoteNodeName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkTlsTrustedCommonNamesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse>> GetDmrClusterLinkTlsTrustedCommonNamesAsyncWithHttpInfo (string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonNames");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->GetDmrClusterLinkTlsTrustedCommonNames");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinkTlsTrustedCommonNames", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkTlsTrustedCommonNamesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkTlsTrustedCommonNamesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkTlsTrustedCommonNamesResponse)));
        }

        /// <summary>
        /// Get a list of Link objects. Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinksResponse</returns>
        public DmrClusterLinksResponse GetDmrClusterLinks (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinksResponse> localVarResponse = GetDmrClusterLinksWithHttpInfo(dmrClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Link objects. Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinksResponse</returns>
        public ApiResponse< DmrClusterLinksResponse > GetDmrClusterLinksWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinks");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinks", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinksResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinksResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinksResponse)));
        }

        /// <summary>
        /// Get a list of Link objects. Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinksResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinksResponse> GetDmrClusterLinksAsync (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinksResponse> localVarResponse = await GetDmrClusterLinksAsyncWithHttpInfo(dmrClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Link objects. Get a list of Link objects.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x dmrClusterName|x||| remoteNodeName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinksResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinksResponse>> GetDmrClusterLinksAsyncWithHttpInfo (string dmrClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->GetDmrClusterLinks");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusterLinks", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinksResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinksResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinksResponse)));
        }

        /// <summary>
        /// Get a list of Cluster objects. Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClustersResponse</returns>
        public DmrClustersResponse GetDmrClusters (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClustersResponse> localVarResponse = GetDmrClustersWithHttpInfo(count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Cluster objects. Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClustersResponse</returns>
        public ApiResponse< DmrClustersResponse > GetDmrClustersWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {

            var localVarPath = "./dmrClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClustersResponse)));
        }

        /// <summary>
        /// Get a list of Cluster objects. Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClustersResponse</returns>
        public async System.Threading.Tasks.Task<DmrClustersResponse> GetDmrClustersAsync (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<DmrClustersResponse> localVarResponse = await GetDmrClustersAsyncWithHttpInfo(count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Cluster objects. Get a list of Cluster objects.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationBasicPassword||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| dmrClusterName|x||| tlsServerCertEnforceTrustedCommonNameEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClustersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClustersResponse>> GetDmrClustersAsyncWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {

            var localVarPath = "./dmrClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetDmrClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClustersResponse)));
        }

        /// <summary>
        /// Replace a Cluster object. Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        public DmrClusterResponse ReplaceDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = ReplaceDmrClusterWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Cluster object. Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        public ApiResponse< DmrClusterResponse > ReplaceDmrClusterWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrCluster");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Replace a Cluster object. Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterResponse> ReplaceDmrClusterAsync (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = await ReplaceDmrClusterAsyncWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Cluster object. Replace a Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| directOnlyEnabled||x|||||| dmrClusterName|x||x||||| nodeName|||x||||| tlsServerCertEnforceTrustedCommonNameEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> ReplaceDmrClusterAsyncWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrCluster");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Replace a Certificate Matching Rule object. Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        public DmrClusterCertMatchingRuleResponse ReplaceDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = ReplaceDmrClusterCertMatchingRuleWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Certificate Matching Rule object. Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleResponse > ReplaceDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Replace a Certificate Matching Rule object. Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> ReplaceDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = await ReplaceDmrClusterCertMatchingRuleAsyncWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Certificate Matching Rule object. Replace a Certificate Matching Rule object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> ReplaceDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object. Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public DmrClusterCertMatchingRuleAttributeFilterResponse ReplaceDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = ReplaceDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object. Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleAttributeFilterResponse > ReplaceDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object. Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> ReplaceDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = await ReplaceDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Certificate Matching Rule Attribute Filter object. Replace a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x||x||||| filterName|x||x||||| ruleName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> ReplaceDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->ReplaceDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Replace a Link object. Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        public DmrClusterLinkResponse ReplaceDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = ReplaceDmrClusterLinkWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Link object. Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        public ApiResponse< DmrClusterLinkResponse > ReplaceDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->ReplaceDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Replace a Link object. Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkResponse> ReplaceDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = await ReplaceDmrClusterLinkAsyncWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Link object. Replace a Link object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword||||x||x||x authenticationScheme||||||x|| dmrClusterName|x||x||||| egressFlowWindowSize||||||x|| initiator||||||x|| remoteNodeName|x||x||||| span||||||x|| transportCompressedEnabled||||||x|| transportTlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> ReplaceDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->ReplaceDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->ReplaceDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->ReplaceDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Update a Cluster object. Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterResponse</returns>
        public DmrClusterResponse UpdateDmrCluster (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = UpdateDmrClusterWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Cluster object. Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterResponse</returns>
        public ApiResponse< DmrClusterResponse > UpdateDmrClusterWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrCluster");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Update a Cluster object. Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterResponse> UpdateDmrClusterAsync (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterResponse> localVarResponse = await UpdateDmrClusterAsyncWithHttpInfo(body, dmrClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Cluster object. Update a Cluster object. Any attribute missing from the request will be left unchanged.  A Cluster is a provisioned object on a message broker that contains global DMR configuration parameters.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| directOnlyEnabled||x||||| dmrClusterName|x|x||||| nodeName||x||||| tlsServerCertEnforceTrustedCommonNameEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- DmrCluster|authenticationClientCertPassword|authenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cluster object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterResponse>> UpdateDmrClusterAsyncWithHttpInfo (DmrCluster body, string dmrClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrCluster");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrCluster");

            var localVarPath = "./dmrClusters/{dmrClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterResponse)));
        }

        /// <summary>
        /// Update a Certificate Matching Rule object. Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleResponse</returns>
        public DmrClusterCertMatchingRuleResponse UpdateDmrClusterCertMatchingRule (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = UpdateDmrClusterCertMatchingRuleWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Certificate Matching Rule object. Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleResponse > UpdateDmrClusterCertMatchingRuleWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Update a Certificate Matching Rule object. Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleResponse> UpdateDmrClusterCertMatchingRuleAsync (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleResponse> localVarResponse = await UpdateDmrClusterCertMatchingRuleAsyncWithHttpInfo(body, dmrClusterName, ruleName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Certificate Matching Rule object. Update a Certificate Matching Rule object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule is a collection of conditions and attribute filters that all have to be satisfied for certificate to be acceptable as authentication for a given link.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleResponse>> UpdateDmrClusterCertMatchingRuleAsyncWithHttpInfo (DmrClusterCertMatchingRule body, string dmrClusterName, string ruleName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRule");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterCertMatchingRule", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleResponse)));
        }

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object. Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public DmrClusterCertMatchingRuleAttributeFilterResponse UpdateDmrClusterCertMatchingRuleAttributeFilter (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = UpdateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object. Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public ApiResponse< DmrClusterCertMatchingRuleAttributeFilterResponse > UpdateDmrClusterCertMatchingRuleAttributeFilterWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object. Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterCertMatchingRuleAttributeFilterResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterCertMatchingRuleAttributeFilterResponse> UpdateDmrClusterCertMatchingRuleAttributeFilterAsync (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse> localVarResponse = await UpdateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo(body, dmrClusterName, ruleName, filterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Certificate Matching Rule Attribute Filter object. Update a Certificate Matching Rule Attribute Filter object. Any attribute missing from the request will be left unchanged.  A Cert Matching Rule Attribute Filter compares a link attribute to a string.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- dmrClusterName|x|x||||| filterName|x|x||||| ruleName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.28.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Certificate Matching Rule Attribute Filter object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="ruleName">The name of the rule.</param>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterCertMatchingRuleAttributeFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>> UpdateDmrClusterCertMatchingRuleAttributeFilterAsyncWithHttpInfo (DmrClusterCertMatchingRuleAttributeFilter body, string dmrClusterName, string ruleName, string filterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'ruleName' is set
            if (ruleName == null)
                throw new ApiException(400, "Missing required parameter 'ruleName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");
            // verify the required parameter 'filterName' is set
            if (filterName == null)
                throw new ApiException(400, "Missing required parameter 'filterName' when calling DmrClusterApi->UpdateDmrClusterCertMatchingRuleAttributeFilter");

            var localVarPath = "./dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (ruleName != null) localVarPathParams.Add("ruleName", this.Configuration.ApiClient.ParameterToString(ruleName)); // path parameter
            if (filterName != null) localVarPathParams.Add("filterName", this.Configuration.ApiClient.ParameterToString(filterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterCertMatchingRuleAttributeFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterCertMatchingRuleAttributeFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterCertMatchingRuleAttributeFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterCertMatchingRuleAttributeFilterResponse)));
        }

        /// <summary>
        /// Update a Link object. Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>DmrClusterLinkResponse</returns>
        public DmrClusterLinkResponse UpdateDmrClusterLink (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = UpdateDmrClusterLinkWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Link object. Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of DmrClusterLinkResponse</returns>
        public ApiResponse< DmrClusterLinkResponse > UpdateDmrClusterLinkWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->UpdateDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

        /// <summary>
        /// Update a Link object. Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of DmrClusterLinkResponse</returns>
        public async System.Threading.Tasks.Task<DmrClusterLinkResponse> UpdateDmrClusterLinkAsync (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<DmrClusterLinkResponse> localVarResponse = await UpdateDmrClusterLinkAsyncWithHttpInfo(body, dmrClusterName, remoteNodeName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Link object. Update a Link object. Any attribute missing from the request will be left unchanged.  A Link connects nodes (either within a Cluster or between two different Clusters) and allows them to exchange topology information, subscriptions and data.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationBasicPassword|||x||x||x authenticationScheme|||||x|| dmrClusterName|x|x||||| egressFlowWindowSize|||||x|| initiator|||||x|| remoteNodeName|x|x||||| span|||||x|| transportCompressedEnabled|||||x|| transportTlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Link object&#x27;s attributes.</param>
        /// <param name="dmrClusterName">The name of the Cluster.</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (DmrClusterLinkResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<DmrClusterLinkResponse>> UpdateDmrClusterLinkAsyncWithHttpInfo (DmrClusterLink body, string dmrClusterName, string remoteNodeName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DmrClusterApi->UpdateDmrClusterLink");
            // verify the required parameter 'dmrClusterName' is set
            if (dmrClusterName == null)
                throw new ApiException(400, "Missing required parameter 'dmrClusterName' when calling DmrClusterApi->UpdateDmrClusterLink");
            // verify the required parameter 'remoteNodeName' is set
            if (remoteNodeName == null)
                throw new ApiException(400, "Missing required parameter 'remoteNodeName' when calling DmrClusterApi->UpdateDmrClusterLink");

            var localVarPath = "./dmrClusters/{dmrClusterName}/links/{remoteNodeName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (dmrClusterName != null) localVarPathParams.Add("dmrClusterName", this.Configuration.ApiClient.ParameterToString(dmrClusterName)); // path parameter
            if (remoteNodeName != null) localVarPathParams.Add("remoteNodeName", this.Configuration.ApiClient.ParameterToString(remoteNodeName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateDmrClusterLink", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<DmrClusterLinkResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (DmrClusterLinkResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(DmrClusterLinkResponse)));
        }

    }
}
