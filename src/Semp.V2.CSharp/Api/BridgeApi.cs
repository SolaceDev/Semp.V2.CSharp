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
        public interface IBridgeApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create a Bridge object.
        /// </summary>
        /// <remarks>
        /// Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        MsgVpnBridgeResponse CreateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Bridge object.
        /// </summary>
        /// <remarks>
        /// Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        ApiResponse<MsgVpnBridgeResponse> CreateMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        MsgVpnBridgeRemoteMsgVpnResponse CreateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> CreateMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionResponse</returns>
        MsgVpnBridgeRemoteSubscriptionResponse CreateMsgVpnBridgeRemoteSubscription (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> CreateMsgVpnBridgeRemoteSubscriptionWithHttpInfo (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        MsgVpnBridgeTlsTrustedCommonNameResponse CreateMsgVpnBridgeTlsTrustedCommonName (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> CreateMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Bridge object.
        /// </summary>
        /// <remarks>
        /// Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter);

        /// <summary>
        /// Delete a Bridge object.
        /// </summary>
        /// <remarks>
        /// Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnBridgeWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter);
        /// <summary>
        /// Delete a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface);

        /// <summary>
        /// Delete a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteMsgVpnWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface);
        /// <summary>
        /// Delete a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic);

        /// <summary>
        /// Delete a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteSubscriptionWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic);
        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName);

        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName);
        /// <summary>
        /// Get a Bridge object.
        /// </summary>
        /// <remarks>
        /// Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        MsgVpnBridgeResponse GetMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Bridge object.
        /// </summary>
        /// <remarks>
        /// Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        ApiResponse<MsgVpnBridgeResponse> GetMsgVpnBridgeWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        MsgVpnBridgeRemoteMsgVpnResponse GetMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> GetMsgVpnBridgeRemoteMsgVpnWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Message VPN objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        MsgVpnBridgeRemoteMsgVpnsResponse GetMsgVpnBridgeRemoteMsgVpns (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Message VPN objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse> GetMsgVpnBridgeRemoteMsgVpnsWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionResponse</returns>
        MsgVpnBridgeRemoteSubscriptionResponse GetMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> GetMsgVpnBridgeRemoteSubscriptionWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        MsgVpnBridgeRemoteSubscriptionsResponse GetMsgVpnBridgeRemoteSubscriptions (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse> GetMsgVpnBridgeRemoteSubscriptionsWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        MsgVpnBridgeTlsTrustedCommonNameResponse GetMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> GetMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        MsgVpnBridgeTlsTrustedCommonNamesResponse GetMsgVpnBridgeTlsTrustedCommonNames (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse> GetMsgVpnBridgeTlsTrustedCommonNamesWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Bridge objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgesResponse</returns>
        MsgVpnBridgesResponse GetMsgVpnBridges (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Bridge objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgesResponse</returns>
        ApiResponse<MsgVpnBridgesResponse> GetMsgVpnBridgesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Bridge object.
        /// </summary>
        /// <remarks>
        /// Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        MsgVpnBridgeResponse ReplaceMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Bridge object.
        /// </summary>
        /// <remarks>
        /// Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        ApiResponse<MsgVpnBridgeResponse> ReplaceMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        MsgVpnBridgeRemoteMsgVpnResponse ReplaceMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> ReplaceMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Bridge object.
        /// </summary>
        /// <remarks>
        /// Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        MsgVpnBridgeResponse UpdateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Bridge object.
        /// </summary>
        /// <remarks>
        /// Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        ApiResponse<MsgVpnBridgeResponse> UpdateMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        MsgVpnBridgeRemoteMsgVpnResponse UpdateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> UpdateMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create a Bridge object.
        /// </summary>
        /// <remarks>
        /// Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeResponse> CreateMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Bridge object.
        /// </summary>
        /// <remarks>
        /// Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> CreateMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> CreateMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> CreateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionResponse> CreateMsgVpnBridgeRemoteSubscriptionAsync (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>> CreateMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNameResponse> CreateMsgVpnBridgeTlsTrustedCommonNameAsync (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNameResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>> CreateMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Bridge object.
        /// </summary>
        /// <remarks>
        /// Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter);

        /// <summary>
        /// Delete a Bridge object.
        /// </summary>
        /// <remarks>
        /// Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter);
        /// <summary>
        /// Delete a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteMsgVpnAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface);

        /// <summary>
        /// Delete a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface);
        /// <summary>
        /// Delete a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteSubscriptionAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic);

        /// <summary>
        /// Delete a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic);
        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeTlsTrustedCommonNameAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName);

        /// <summary>
        /// Delete a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName);
        /// <summary>
        /// Get a Bridge object.
        /// </summary>
        /// <remarks>
        /// Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeResponse> GetMsgVpnBridgeAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Bridge object.
        /// </summary>
        /// <remarks>
        /// Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> GetMsgVpnBridgeAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> GetMsgVpnBridgeRemoteMsgVpnAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> GetMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Message VPN objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnsResponse> GetMsgVpnBridgeRemoteMsgVpnsAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Message VPN objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse>> GetMsgVpnBridgeRemoteMsgVpnsAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionResponse> GetMsgVpnBridgeRemoteSubscriptionAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Remote Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>> GetMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Remote Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionsResponse> GetMsgVpnBridgeRemoteSubscriptionsAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Remote Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse>> GetMsgVpnBridgeRemoteSubscriptionsAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNameResponse> GetMsgVpnBridgeTlsTrustedCommonNameAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trusted Common Name object.
        /// </summary>
        /// <remarks>
        /// Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNameResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>> GetMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNamesResponse> GetMsgVpnBridgeTlsTrustedCommonNamesAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trusted Common Name objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNamesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse>> GetMsgVpnBridgeTlsTrustedCommonNamesAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Bridge objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgesResponse> GetMsgVpnBridgesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Bridge objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgesResponse>> GetMsgVpnBridgesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Bridge object.
        /// </summary>
        /// <remarks>
        /// Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeResponse> ReplaceMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Bridge object.
        /// </summary>
        /// <remarks>
        /// Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> ReplaceMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> ReplaceMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> ReplaceMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Bridge object.
        /// </summary>
        /// <remarks>
        /// Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeResponse> UpdateMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Bridge object.
        /// </summary>
        /// <remarks>
        /// Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> UpdateMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> UpdateMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Remote Message VPN object.
        /// </summary>
        /// <remarks>
        /// Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> UpdateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class BridgeApi : IBridgeApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeApi"/> class.
        /// </summary>
        /// <returns></returns>
        public BridgeApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeApi"/> class
        /// </summary>
        /// <returns></returns>
        public BridgeApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public BridgeApi(Semp.V2.CSharp.Client.Configuration configuration = null)
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
        /// Create a Bridge object. Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        public MsgVpnBridgeResponse CreateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = CreateMsgVpnBridgeWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Bridge object. Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        public ApiResponse< MsgVpnBridgeResponse > CreateMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Create a Bridge object. Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeResponse> CreateMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = await CreateMsgVpnBridgeAsyncWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Bridge object. Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> CreateMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Create a Remote Message VPN object. Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public MsgVpnBridgeRemoteMsgVpnResponse CreateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = CreateMsgVpnBridgeRemoteMsgVpnWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Remote Message VPN object. Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteMsgVpnResponse > CreateMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Create a Remote Message VPN object. Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> CreateMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = await CreateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Remote Message VPN object. Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> CreateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Create a Remote Subscription object. Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public MsgVpnBridgeRemoteSubscriptionResponse CreateMsgVpnBridgeRemoteSubscription (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> localVarResponse = CreateMsgVpnBridgeRemoteSubscriptionWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Remote Subscription object. Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteSubscriptionResponse > CreateMsgVpnBridgeRemoteSubscriptionWithHttpInfo (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionResponse)));
        }

        /// <summary>
        /// Create a Remote Subscription object. Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionResponse> CreateMsgVpnBridgeRemoteSubscriptionAsync (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> localVarResponse = await CreateMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Remote Subscription object. Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>> CreateMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionResponse)));
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public MsgVpnBridgeTlsTrustedCommonNameResponse CreateMsgVpnBridgeTlsTrustedCommonName (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> localVarResponse = CreateMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public ApiResponse< MsgVpnBridgeTlsTrustedCommonNameResponse > CreateMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNameResponse> CreateMsgVpnBridgeTlsTrustedCommonNameAsync (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> localVarResponse = await CreateMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Trusted Common Name object. Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trusted Common Name object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNameResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>> CreateMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->CreateMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Delete a Bridge object. Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnBridgeWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Bridge object. Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnBridgeWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Bridge object. Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnBridgeAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Bridge object. Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Message VPN object. Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnBridgeRemoteMsgVpnWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Remote Message VPN object. Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnBridgeRemoteMsgVpnWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Message VPN object. Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteMsgVpnAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Remote Message VPN object. Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->DeleteMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Subscription object. Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnBridgeRemoteSubscriptionWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Remote Subscription object. Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnBridgeRemoteSubscriptionWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'remoteSubscriptionTopic' is set
            if (remoteSubscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'remoteSubscriptionTopic' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteSubscriptionTopic != null) localVarPathParams.Add("remoteSubscriptionTopic", this.Configuration.ApiClient.ParameterToString(remoteSubscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Remote Subscription object. Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeRemoteSubscriptionAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Remote Subscription object. Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'remoteSubscriptionTopic' is set
            if (remoteSubscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'remoteSubscriptionTopic' when calling BridgeApi->DeleteMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteSubscriptionTopic != null) localVarPathParams.Add("remoteSubscriptionTopic", this.Configuration.ApiClient.ParameterToString(remoteSubscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnBridgeTlsTrustedCommonNameAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Trusted Common Name object. Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling BridgeApi->DeleteMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get a Bridge object. Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        public MsgVpnBridgeResponse GetMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = GetMsgVpnBridgeWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Bridge object. Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        public ApiResponse< MsgVpnBridgeResponse > GetMsgVpnBridgeWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Get a Bridge object. Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeResponse> GetMsgVpnBridgeAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = await GetMsgVpnBridgeAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Bridge object. Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> GetMsgVpnBridgeAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Get a Remote Message VPN object. Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public MsgVpnBridgeRemoteMsgVpnResponse GetMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = GetMsgVpnBridgeRemoteMsgVpnWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Remote Message VPN object. Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteMsgVpnResponse > GetMsgVpnBridgeRemoteMsgVpnWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Get a Remote Message VPN object. Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> GetMsgVpnBridgeRemoteMsgVpnAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = await GetMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Remote Message VPN object. Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> GetMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Get a list of Remote Message VPN objects. Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        public MsgVpnBridgeRemoteMsgVpnsResponse GetMsgVpnBridgeRemoteMsgVpns (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse> localVarResponse = GetMsgVpnBridgeRemoteMsgVpnsWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Remote Message VPN objects. Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteMsgVpnsResponse > GetMsgVpnBridgeRemoteMsgVpnsWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteMsgVpns", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnsResponse)));
        }

        /// <summary>
        /// Get a list of Remote Message VPN objects. Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnsResponse> GetMsgVpnBridgeRemoteMsgVpnsAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse> localVarResponse = await GetMsgVpnBridgeRemoteMsgVpnsAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Remote Message VPN objects. Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse>> GetMsgVpnBridgeRemoteMsgVpnsAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteMsgVpns");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteMsgVpns", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnsResponse)));
        }

        /// <summary>
        /// Get a Remote Subscription object. Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public MsgVpnBridgeRemoteSubscriptionResponse GetMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> localVarResponse = GetMsgVpnBridgeRemoteSubscriptionWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Remote Subscription object. Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteSubscriptionResponse > GetMsgVpnBridgeRemoteSubscriptionWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'remoteSubscriptionTopic' is set
            if (remoteSubscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'remoteSubscriptionTopic' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteSubscriptionTopic != null) localVarPathParams.Add("remoteSubscriptionTopic", this.Configuration.ApiClient.ParameterToString(remoteSubscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionResponse)));
        }

        /// <summary>
        /// Get a Remote Subscription object. Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionResponse> GetMsgVpnBridgeRemoteSubscriptionAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse> localVarResponse = await GetMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Remote Subscription object. Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteSubscriptionTopic">The topic of the Bridge remote subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>> GetMsgVpnBridgeRemoteSubscriptionAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");
            // verify the required parameter 'remoteSubscriptionTopic' is set
            if (remoteSubscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'remoteSubscriptionTopic' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteSubscriptionTopic != null) localVarPathParams.Add("remoteSubscriptionTopic", this.Configuration.ApiClient.ParameterToString(remoteSubscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionResponse)));
        }

        /// <summary>
        /// Get a list of Remote Subscription objects. Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        public MsgVpnBridgeRemoteSubscriptionsResponse GetMsgVpnBridgeRemoteSubscriptions (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse> localVarResponse = GetMsgVpnBridgeRemoteSubscriptionsWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Remote Subscription objects. Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteSubscriptionsResponse > GetMsgVpnBridgeRemoteSubscriptionsWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a list of Remote Subscription objects. Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteSubscriptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteSubscriptionsResponse> GetMsgVpnBridgeRemoteSubscriptionsAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse> localVarResponse = await GetMsgVpnBridgeRemoteSubscriptionsAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Remote Subscription objects. Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteSubscriptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse>> GetMsgVpnBridgeRemoteSubscriptionsAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeRemoteSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeRemoteSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public MsgVpnBridgeTlsTrustedCommonNameResponse GetMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> localVarResponse = GetMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public ApiResponse< MsgVpnBridgeTlsTrustedCommonNameResponse > GetMsgVpnBridgeTlsTrustedCommonNameWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNameResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNameResponse> GetMsgVpnBridgeTlsTrustedCommonNameAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse> localVarResponse = await GetMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Trusted Common Name object. Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="tlsTrustedCommonName">The expected trusted common name of the remote certificate.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNameResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>> GetMsgVpnBridgeTlsTrustedCommonNameAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");
            // verify the required parameter 'tlsTrustedCommonName' is set
            if (tlsTrustedCommonName == null)
                throw new ApiException(400, "Missing required parameter 'tlsTrustedCommonName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonName");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeTlsTrustedCommonName", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNameResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNameResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNameResponse)));
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        public MsgVpnBridgeTlsTrustedCommonNamesResponse GetMsgVpnBridgeTlsTrustedCommonNames (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse> localVarResponse = GetMsgVpnBridgeTlsTrustedCommonNamesWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        public ApiResponse< MsgVpnBridgeTlsTrustedCommonNamesResponse > GetMsgVpnBridgeTlsTrustedCommonNamesWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeTlsTrustedCommonNames", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNamesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNamesResponse)));
        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeTlsTrustedCommonNamesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeTlsTrustedCommonNamesResponse> GetMsgVpnBridgeTlsTrustedCommonNamesAsync (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse> localVarResponse = await GetMsgVpnBridgeTlsTrustedCommonNamesAsyncWithHttpInfo(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Trusted Common Name objects. Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node&#x27;s server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeTlsTrustedCommonNamesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse>> GetMsgVpnBridgeTlsTrustedCommonNamesAsyncWithHttpInfo (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->GetMsgVpnBridgeTlsTrustedCommonNames");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridgeTlsTrustedCommonNames", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeTlsTrustedCommonNamesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeTlsTrustedCommonNamesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeTlsTrustedCommonNamesResponse)));
        }

        /// <summary>
        /// Get a list of Bridge objects. Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgesResponse</returns>
        public MsgVpnBridgesResponse GetMsgVpnBridges (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgesResponse> localVarResponse = GetMsgVpnBridgesWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Bridge objects. Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgesResponse</returns>
        public ApiResponse< MsgVpnBridgesResponse > GetMsgVpnBridgesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridges");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridges", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgesResponse)));
        }

        /// <summary>
        /// Get a list of Bridge objects. Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgesResponse> GetMsgVpnBridgesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgesResponse> localVarResponse = await GetMsgVpnBridgesAsyncWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Bridge objects. Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgesResponse>> GetMsgVpnBridgesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->GetMsgVpnBridges");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnBridges", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgesResponse)));
        }

        /// <summary>
        /// Replace a Bridge object. Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        public MsgVpnBridgeResponse ReplaceMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = ReplaceMsgVpnBridgeWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Bridge object. Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        public ApiResponse< MsgVpnBridgeResponse > ReplaceMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->ReplaceMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Replace a Bridge object. Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeResponse> ReplaceMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = await ReplaceMsgVpnBridgeAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Bridge object. Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> ReplaceMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->ReplaceMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->ReplaceMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Replace a Remote Message VPN object. Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public MsgVpnBridgeRemoteMsgVpnResponse ReplaceMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = ReplaceMsgVpnBridgeRemoteMsgVpnWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Remote Message VPN object. Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteMsgVpnResponse > ReplaceMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Replace a Remote Message VPN object. Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> ReplaceMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = await ReplaceMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Remote Message VPN object. Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> ReplaceMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->ReplaceMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Update a Bridge object. Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeResponse</returns>
        public MsgVpnBridgeResponse UpdateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = UpdateMsgVpnBridgeWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Bridge object. Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeResponse</returns>
        public ApiResponse< MsgVpnBridgeResponse > UpdateMsgVpnBridgeWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->UpdateMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Update a Bridge object. Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeResponse> UpdateMsgVpnBridgeAsync (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeResponse> localVarResponse = await UpdateMsgVpnBridgeAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Bridge object. Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Bridge object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeResponse>> UpdateMsgVpnBridgeAsyncWithHttpInfo (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->UpdateMsgVpnBridge");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->UpdateMsgVpnBridge");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnBridge", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeResponse)));
        }

        /// <summary>
        /// Update a Remote Message VPN object. Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public MsgVpnBridgeRemoteMsgVpnResponse UpdateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = UpdateMsgVpnBridgeRemoteMsgVpnWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Remote Message VPN object. Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public ApiResponse< MsgVpnBridgeRemoteMsgVpnResponse > UpdateMsgVpnBridgeRemoteMsgVpnWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

        /// <summary>
        /// Update a Remote Message VPN object. Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnBridgeRemoteMsgVpnResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnBridgeRemoteMsgVpnResponse> UpdateMsgVpnBridgeRemoteMsgVpnAsync (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse> localVarResponse = await UpdateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Remote Message VPN object. Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Remote Message VPN object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="bridgeName">The name of the Bridge.</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge.</param>
        /// <param name="remoteMsgVpnName">The name of the remote Message VPN.</param>
        /// <param name="remoteMsgVpnLocation">The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;).</param>
        /// <param name="remoteMsgVpnInterface">The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnBridgeRemoteMsgVpnResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>> UpdateMsgVpnBridgeRemoteMsgVpnAsyncWithHttpInfo (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeName' is set
            if (bridgeName == null)
                throw new ApiException(400, "Missing required parameter 'bridgeName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'bridgeVirtualRouter' is set
            if (bridgeVirtualRouter == null)
                throw new ApiException(400, "Missing required parameter 'bridgeVirtualRouter' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnName' is set
            if (remoteMsgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnName' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnLocation' is set
            if (remoteMsgVpnLocation == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnLocation' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");
            // verify the required parameter 'remoteMsgVpnInterface' is set
            if (remoteMsgVpnInterface == null)
                throw new ApiException(400, "Missing required parameter 'remoteMsgVpnInterface' when calling BridgeApi->UpdateMsgVpnBridgeRemoteMsgVpn");

            var localVarPath = "./msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (bridgeName != null) localVarPathParams.Add("bridgeName", this.Configuration.ApiClient.ParameterToString(bridgeName)); // path parameter
            if (bridgeVirtualRouter != null) localVarPathParams.Add("bridgeVirtualRouter", this.Configuration.ApiClient.ParameterToString(bridgeVirtualRouter)); // path parameter
            if (remoteMsgVpnName != null) localVarPathParams.Add("remoteMsgVpnName", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnName)); // path parameter
            if (remoteMsgVpnLocation != null) localVarPathParams.Add("remoteMsgVpnLocation", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnLocation)); // path parameter
            if (remoteMsgVpnInterface != null) localVarPathParams.Add("remoteMsgVpnInterface", this.Configuration.ApiClient.ParameterToString(remoteMsgVpnInterface)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnBridgeRemoteMsgVpn", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnBridgeRemoteMsgVpnResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnBridgeRemoteMsgVpnResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnBridgeRemoteMsgVpnResponse)));
        }

    }
}
