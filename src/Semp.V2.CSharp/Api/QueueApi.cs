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
        public interface IQueueApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create a Queue object.
        /// </summary>
        /// <remarks>
        /// Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        MsgVpnQueueResponse CreateMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Queue object.
        /// </summary>
        /// <remarks>
        /// Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        ApiResponse<MsgVpnQueueResponse> CreateMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionResponse</returns>
        MsgVpnQueueSubscriptionResponse CreateMsgVpnQueueSubscription (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionResponse</returns>
        ApiResponse<MsgVpnQueueSubscriptionResponse> CreateMsgVpnQueueSubscriptionWithHttpInfo (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Queue object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnQueue (string msgVpnName, string queueName);

        /// <summary>
        /// Delete a Queue object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnQueueWithHttpInfo (string msgVpnName, string queueName);
        /// <summary>
        /// Delete a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnQueueSubscription (string msgVpnName, string queueName, string subscriptionTopic);

        /// <summary>
        /// Delete a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnQueueSubscriptionWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic);
        /// <summary>
        /// Get a Queue object.
        /// </summary>
        /// <remarks>
        /// Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        MsgVpnQueueResponse GetMsgVpnQueue (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Queue object.
        /// </summary>
        /// <remarks>
        /// Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        ApiResponse<MsgVpnQueueResponse> GetMsgVpnQueueWithHttpInfo (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionResponse</returns>
        MsgVpnQueueSubscriptionResponse GetMsgVpnQueueSubscription (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionResponse</returns>
        ApiResponse<MsgVpnQueueSubscriptionResponse> GetMsgVpnQueueSubscriptionWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Queue Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionsResponse</returns>
        MsgVpnQueueSubscriptionsResponse GetMsgVpnQueueSubscriptions (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Queue Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionsResponse</returns>
        ApiResponse<MsgVpnQueueSubscriptionsResponse> GetMsgVpnQueueSubscriptionsWithHttpInfo (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Queue objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueuesResponse</returns>
        MsgVpnQueuesResponse GetMsgVpnQueues (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Queue objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueuesResponse</returns>
        ApiResponse<MsgVpnQueuesResponse> GetMsgVpnQueuesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Queue object.
        /// </summary>
        /// <remarks>
        /// Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        MsgVpnQueueResponse ReplaceMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Queue object.
        /// </summary>
        /// <remarks>
        /// Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        ApiResponse<MsgVpnQueueResponse> ReplaceMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Queue object.
        /// </summary>
        /// <remarks>
        /// Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        MsgVpnQueueResponse UpdateMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Queue object.
        /// </summary>
        /// <remarks>
        /// Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        ApiResponse<MsgVpnQueueResponse> UpdateMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create a Queue object.
        /// </summary>
        /// <remarks>
        /// Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueResponse> CreateMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Queue object.
        /// </summary>
        /// <remarks>
        /// Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> CreateMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueSubscriptionResponse> CreateMsgVpnQueueSubscriptionAsync (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionResponse>> CreateMsgVpnQueueSubscriptionAsyncWithHttpInfo (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Queue object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnQueueAsync (string msgVpnName, string queueName);

        /// <summary>
        /// Delete a Queue object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnQueueAsyncWithHttpInfo (string msgVpnName, string queueName);
        /// <summary>
        /// Delete a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnQueueSubscriptionAsync (string msgVpnName, string queueName, string subscriptionTopic);

        /// <summary>
        /// Delete a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnQueueSubscriptionAsyncWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic);
        /// <summary>
        /// Get a Queue object.
        /// </summary>
        /// <remarks>
        /// Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueResponse> GetMsgVpnQueueAsync (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Queue object.
        /// </summary>
        /// <remarks>
        /// Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> GetMsgVpnQueueAsyncWithHttpInfo (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueSubscriptionResponse> GetMsgVpnQueueSubscriptionAsync (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Queue Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionResponse>> GetMsgVpnQueueSubscriptionAsyncWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Queue Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueSubscriptionsResponse> GetMsgVpnQueueSubscriptionsAsync (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Queue Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionsResponse>> GetMsgVpnQueueSubscriptionsAsyncWithHttpInfo (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Queue objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueuesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueuesResponse> GetMsgVpnQueuesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Queue objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueuesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueuesResponse>> GetMsgVpnQueuesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Queue object.
        /// </summary>
        /// <remarks>
        /// Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueResponse> ReplaceMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Queue object.
        /// </summary>
        /// <remarks>
        /// Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> ReplaceMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Queue object.
        /// </summary>
        /// <remarks>
        /// Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        System.Threading.Tasks.Task<MsgVpnQueueResponse> UpdateMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Queue object.
        /// </summary>
        /// <remarks>
        /// Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> UpdateMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class QueueApi : IQueueApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueApi"/> class.
        /// </summary>
        /// <returns></returns>
        public QueueApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueApi"/> class
        /// </summary>
        /// <returns></returns>
        public QueueApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public QueueApi(Semp.V2.CSharp.Client.Configuration configuration = null)
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
        /// Create a Queue object. Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        public MsgVpnQueueResponse CreateMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = CreateMsgVpnQueueWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Queue object. Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        public ApiResponse< MsgVpnQueueResponse > CreateMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->CreateMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->CreateMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues";
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
                Exception exception = ExceptionFactory("CreateMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Create a Queue object. Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueResponse> CreateMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = await CreateMsgVpnQueueAsyncWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Queue object. Create a Queue object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> CreateMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->CreateMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->CreateMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues";
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
                Exception exception = ExceptionFactory("CreateMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Create a Queue Subscription object. Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionResponse</returns>
        public MsgVpnQueueSubscriptionResponse CreateMsgVpnQueueSubscription (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionResponse> localVarResponse = CreateMsgVpnQueueSubscriptionWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Queue Subscription object. Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionResponse</returns>
        public ApiResponse< MsgVpnQueueSubscriptionResponse > CreateMsgVpnQueueSubscriptionWithHttpInfo (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->CreateMsgVpnQueueSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->CreateMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->CreateMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionResponse)));
        }

        /// <summary>
        /// Create a Queue Subscription object. Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueSubscriptionResponse> CreateMsgVpnQueueSubscriptionAsync (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionResponse> localVarResponse = await CreateMsgVpnQueueSubscriptionAsyncWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Queue Subscription object. Create a Queue Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueName|x||x||| subscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionResponse>> CreateMsgVpnQueueSubscriptionAsyncWithHttpInfo (MsgVpnQueueSubscription body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->CreateMsgVpnQueueSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->CreateMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->CreateMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionResponse)));
        }

        /// <summary>
        /// Delete a Queue object. Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnQueue (string msgVpnName, string queueName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnQueueWithHttpInfo(msgVpnName, queueName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Queue object. Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnQueueWithHttpInfo (string msgVpnName, string queueName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->DeleteMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->DeleteMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Queue object. Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnQueueAsync (string msgVpnName, string queueName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnQueueAsyncWithHttpInfo(msgVpnName, queueName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Queue object. Delete a Queue object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnQueueAsyncWithHttpInfo (string msgVpnName, string queueName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->DeleteMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->DeleteMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Queue Subscription object. Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnQueueSubscription (string msgVpnName, string queueName, string subscriptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnQueueSubscriptionWithHttpInfo(msgVpnName, queueName, subscriptionTopic);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Queue Subscription object. Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnQueueSubscriptionWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->DeleteMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->DeleteMsgVpnQueueSubscription");
            // verify the required parameter 'subscriptionTopic' is set
            if (subscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionTopic' when calling QueueApi->DeleteMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
            if (subscriptionTopic != null) localVarPathParams.Add("subscriptionTopic", this.Configuration.ApiClient.ParameterToString(subscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Queue Subscription object. Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnQueueSubscriptionAsync (string msgVpnName, string queueName, string subscriptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnQueueSubscriptionAsyncWithHttpInfo(msgVpnName, queueName, subscriptionTopic);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Queue Subscription object. Delete a Queue Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnQueueSubscriptionAsyncWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->DeleteMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->DeleteMsgVpnQueueSubscription");
            // verify the required parameter 'subscriptionTopic' is set
            if (subscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionTopic' when calling QueueApi->DeleteMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
            if (subscriptionTopic != null) localVarPathParams.Add("subscriptionTopic", this.Configuration.ApiClient.ParameterToString(subscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get a Queue object. Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        public MsgVpnQueueResponse GetMsgVpnQueue (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = GetMsgVpnQueueWithHttpInfo(msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Queue object. Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        public ApiResponse< MsgVpnQueueResponse > GetMsgVpnQueueWithHttpInfo (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Get a Queue object. Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueResponse> GetMsgVpnQueueAsync (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = await GetMsgVpnQueueAsyncWithHttpInfo(msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Queue object. Get a Queue object.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> GetMsgVpnQueueAsyncWithHttpInfo (string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Get a Queue Subscription object. Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionResponse</returns>
        public MsgVpnQueueSubscriptionResponse GetMsgVpnQueueSubscription (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionResponse> localVarResponse = GetMsgVpnQueueSubscriptionWithHttpInfo(msgVpnName, queueName, subscriptionTopic, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Queue Subscription object. Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionResponse</returns>
        public ApiResponse< MsgVpnQueueSubscriptionResponse > GetMsgVpnQueueSubscriptionWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueueSubscription");
            // verify the required parameter 'subscriptionTopic' is set
            if (subscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionTopic' when calling QueueApi->GetMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
            if (subscriptionTopic != null) localVarPathParams.Add("subscriptionTopic", this.Configuration.ApiClient.ParameterToString(subscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionResponse)));
        }

        /// <summary>
        /// Get a Queue Subscription object. Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueSubscriptionResponse> GetMsgVpnQueueSubscriptionAsync (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionResponse> localVarResponse = await GetMsgVpnQueueSubscriptionAsyncWithHttpInfo(msgVpnName, queueName, subscriptionTopic, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Queue Subscription object. Get a Queue Subscription object.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="subscriptionTopic">The topic of the Subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionResponse>> GetMsgVpnQueueSubscriptionAsyncWithHttpInfo (string msgVpnName, string queueName, string subscriptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueueSubscription");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueueSubscription");
            // verify the required parameter 'subscriptionTopic' is set
            if (subscriptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionTopic' when calling QueueApi->GetMsgVpnQueueSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
            if (subscriptionTopic != null) localVarPathParams.Add("subscriptionTopic", this.Configuration.ApiClient.ParameterToString(subscriptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueueSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionResponse)));
        }

        /// <summary>
        /// Get a list of Queue Subscription objects. Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueSubscriptionsResponse</returns>
        public MsgVpnQueueSubscriptionsResponse GetMsgVpnQueueSubscriptions (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionsResponse> localVarResponse = GetMsgVpnQueueSubscriptionsWithHttpInfo(msgVpnName, queueName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Queue Subscription objects. Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueSubscriptionsResponse</returns>
        public ApiResponse< MsgVpnQueueSubscriptionsResponse > GetMsgVpnQueueSubscriptionsWithHttpInfo (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueueSubscriptions");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueueSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueueSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a list of Queue Subscription objects. Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueSubscriptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueSubscriptionsResponse> GetMsgVpnQueueSubscriptionsAsync (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueSubscriptionsResponse> localVarResponse = await GetMsgVpnQueueSubscriptionsAsyncWithHttpInfo(msgVpnName, queueName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Queue Subscription objects. Get a list of Queue Subscription objects.  One or more Queue Subscriptions can be added to a durable queue so that Guaranteed messages published to matching topics are also delivered to and spooled by the queue.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x||| subscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueSubscriptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueSubscriptionsResponse>> GetMsgVpnQueueSubscriptionsAsyncWithHttpInfo (string msgVpnName, string queueName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueueSubscriptions");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->GetMsgVpnQueueSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}/subscriptions";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnQueueSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a list of Queue objects. Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueuesResponse</returns>
        public MsgVpnQueuesResponse GetMsgVpnQueues (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueuesResponse> localVarResponse = GetMsgVpnQueuesWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Queue objects. Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueuesResponse</returns>
        public ApiResponse< MsgVpnQueuesResponse > GetMsgVpnQueuesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueues");

            var localVarPath = "./msgVpns/{msgVpnName}/queues";
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
                Exception exception = ExceptionFactory("GetMsgVpnQueues", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueuesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueuesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueuesResponse)));
        }

        /// <summary>
        /// Get a list of Queue objects. Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueuesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueuesResponse> GetMsgVpnQueuesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueuesResponse> localVarResponse = await GetMsgVpnQueuesAsyncWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Queue objects. Get a list of Queue objects.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueuesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueuesResponse>> GetMsgVpnQueuesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->GetMsgVpnQueues");

            var localVarPath = "./msgVpns/{msgVpnName}/queues";
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
                Exception exception = ExceptionFactory("GetMsgVpnQueues", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueuesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueuesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueuesResponse)));
        }

        /// <summary>
        /// Replace a Queue object. Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        public MsgVpnQueueResponse ReplaceMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = ReplaceMsgVpnQueueWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Queue object. Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        public ApiResponse< MsgVpnQueueResponse > ReplaceMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->ReplaceMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->ReplaceMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->ReplaceMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Replace a Queue object. Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueResponse> ReplaceMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = await ReplaceMsgVpnQueueAsyncWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Queue object. Replace a Queue object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| queueName|x||x||||| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> ReplaceMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->ReplaceMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->ReplaceMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->ReplaceMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Update a Queue object. Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnQueueResponse</returns>
        public MsgVpnQueueResponse UpdateMsgVpnQueue (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = UpdateMsgVpnQueueWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Queue object. Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnQueueResponse</returns>
        public ApiResponse< MsgVpnQueueResponse > UpdateMsgVpnQueueWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->UpdateMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->UpdateMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->UpdateMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

        /// <summary>
        /// Update a Queue object. Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnQueueResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnQueueResponse> UpdateMsgVpnQueueAsync (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnQueueResponse> localVarResponse = await UpdateMsgVpnQueueAsyncWithHttpInfo(body, msgVpnName, queueName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Queue object. Update a Queue object. Any attribute missing from the request will be left unchanged.  A Queue acts as both a destination that clients can publish messages to, and as an endpoint that clients can bind consumers to and consume messages from.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| queueName|x|x||||| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Queue object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="queueName">The name of the Queue.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnQueueResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnQueueResponse>> UpdateMsgVpnQueueAsyncWithHttpInfo (MsgVpnQueue body, string msgVpnName, string queueName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling QueueApi->UpdateMsgVpnQueue");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling QueueApi->UpdateMsgVpnQueue");
            // verify the required parameter 'queueName' is set
            if (queueName == null)
                throw new ApiException(400, "Missing required parameter 'queueName' when calling QueueApi->UpdateMsgVpnQueue");

            var localVarPath = "./msgVpns/{msgVpnName}/queues/{queueName}";
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
            if (queueName != null) localVarPathParams.Add("queueName", this.Configuration.ApiClient.ParameterToString(queueName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnQueue", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnQueueResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnQueueResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnQueueResponse)));
        }

    }
}
