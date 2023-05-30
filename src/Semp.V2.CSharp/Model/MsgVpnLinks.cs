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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = Semp.V2.CSharp.Client.SwaggerDateConverter;

namespace Semp.V2.CSharp.Model
{
    /// <summary>
    /// MsgVpnLinks
    /// </summary>
    [DataContract]
        public partial class MsgVpnLinks :  IEquatable<MsgVpnLinks>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnLinks" /> class.
        /// </summary>
        /// <param name="aclProfilesUri">The URI of this Message VPN&#x27;s collection of ACL Profile objects..</param>
        /// <param name="authenticationOauthProfilesUri">The URI of this Message VPN&#x27;s collection of OAuth Profile objects. Available since 2.25..</param>
        /// <param name="authenticationOauthProvidersUri">The URI of this Message VPN&#x27;s collection of OAuth Provider objects. Deprecated since 2.25. Replaced by authenticationOauthProfiles..</param>
        /// <param name="authorizationGroupsUri">The URI of this Message VPN&#x27;s collection of Authorization Group objects..</param>
        /// <param name="bridgesUri">The URI of this Message VPN&#x27;s collection of Bridge objects..</param>
        /// <param name="certMatchingRulesUri">The URI of this Message VPN&#x27;s collection of Certificate Matching Rule objects. Available since 2.27..</param>
        /// <param name="clientProfilesUri">The URI of this Message VPN&#x27;s collection of Client Profile objects..</param>
        /// <param name="clientUsernamesUri">The URI of this Message VPN&#x27;s collection of Client Username objects..</param>
        /// <param name="distributedCachesUri">The URI of this Message VPN&#x27;s collection of Distributed Cache objects. Available since 2.11..</param>
        /// <param name="dmrBridgesUri">The URI of this Message VPN&#x27;s collection of DMR Bridge objects. Available since 2.11..</param>
        /// <param name="jndiConnectionFactoriesUri">The URI of this Message VPN&#x27;s collection of JNDI Connection Factory objects. Available since 2.2..</param>
        /// <param name="jndiQueuesUri">The URI of this Message VPN&#x27;s collection of JNDI Queue objects. Available since 2.2..</param>
        /// <param name="jndiTopicsUri">The URI of this Message VPN&#x27;s collection of JNDI Topic objects. Available since 2.2..</param>
        /// <param name="mqttRetainCachesUri">The URI of this Message VPN&#x27;s collection of MQTT Retain Cache objects. Available since 2.11..</param>
        /// <param name="mqttSessionsUri">The URI of this Message VPN&#x27;s collection of MQTT Session objects. Available since 2.1..</param>
        /// <param name="queueTemplatesUri">The URI of this Message VPN&#x27;s collection of Queue Template objects. Available since 2.14..</param>
        /// <param name="queuesUri">The URI of this Message VPN&#x27;s collection of Queue objects..</param>
        /// <param name="replayLogsUri">The URI of this Message VPN&#x27;s collection of Replay Log objects. Available since 2.10..</param>
        /// <param name="replicatedTopicsUri">The URI of this Message VPN&#x27;s collection of Replicated Topic objects. Available since 2.1..</param>
        /// <param name="restDeliveryPointsUri">The URI of this Message VPN&#x27;s collection of REST Delivery Point objects..</param>
        /// <param name="sequencedTopicsUri">The URI of this Message VPN&#x27;s collection of Sequenced Topic objects..</param>
        /// <param name="telemetryProfilesUri">The URI of this Message VPN&#x27;s collection of Telemetry Profile objects. Available since 2.31..</param>
        /// <param name="topicEndpointTemplatesUri">The URI of this Message VPN&#x27;s collection of Topic Endpoint Template objects. Available since 2.14..</param>
        /// <param name="topicEndpointsUri">The URI of this Message VPN&#x27;s collection of Topic Endpoint objects. Available since 2.1..</param>
        /// <param name="uri">The URI of this Message VPN object..</param>
        public MsgVpnLinks(string aclProfilesUri = default(string), string authenticationOauthProfilesUri = default(string), string authenticationOauthProvidersUri = default(string), string authorizationGroupsUri = default(string), string bridgesUri = default(string), string certMatchingRulesUri = default(string), string clientProfilesUri = default(string), string clientUsernamesUri = default(string), string distributedCachesUri = default(string), string dmrBridgesUri = default(string), string jndiConnectionFactoriesUri = default(string), string jndiQueuesUri = default(string), string jndiTopicsUri = default(string), string mqttRetainCachesUri = default(string), string mqttSessionsUri = default(string), string queueTemplatesUri = default(string), string queuesUri = default(string), string replayLogsUri = default(string), string replicatedTopicsUri = default(string), string restDeliveryPointsUri = default(string), string sequencedTopicsUri = default(string), string telemetryProfilesUri = default(string), string topicEndpointTemplatesUri = default(string), string topicEndpointsUri = default(string), string uri = default(string))
        {
            this.AclProfilesUri = aclProfilesUri;
            this.AuthenticationOauthProfilesUri = authenticationOauthProfilesUri;
            this.AuthenticationOauthProvidersUri = authenticationOauthProvidersUri;
            this.AuthorizationGroupsUri = authorizationGroupsUri;
            this.BridgesUri = bridgesUri;
            this.CertMatchingRulesUri = certMatchingRulesUri;
            this.ClientProfilesUri = clientProfilesUri;
            this.ClientUsernamesUri = clientUsernamesUri;
            this.DistributedCachesUri = distributedCachesUri;
            this.DmrBridgesUri = dmrBridgesUri;
            this.JndiConnectionFactoriesUri = jndiConnectionFactoriesUri;
            this.JndiQueuesUri = jndiQueuesUri;
            this.JndiTopicsUri = jndiTopicsUri;
            this.MqttRetainCachesUri = mqttRetainCachesUri;
            this.MqttSessionsUri = mqttSessionsUri;
            this.QueueTemplatesUri = queueTemplatesUri;
            this.QueuesUri = queuesUri;
            this.ReplayLogsUri = replayLogsUri;
            this.ReplicatedTopicsUri = replicatedTopicsUri;
            this.RestDeliveryPointsUri = restDeliveryPointsUri;
            this.SequencedTopicsUri = sequencedTopicsUri;
            this.TelemetryProfilesUri = telemetryProfilesUri;
            this.TopicEndpointTemplatesUri = topicEndpointTemplatesUri;
            this.TopicEndpointsUri = topicEndpointsUri;
            this.Uri = uri;
        }
        
        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of ACL Profile objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of ACL Profile objects.</value>
        [DataMember(Name="aclProfilesUri", EmitDefaultValue=false)]
        public string AclProfilesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of OAuth Profile objects. Available since 2.25.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of OAuth Profile objects. Available since 2.25.</value>
        [DataMember(Name="authenticationOauthProfilesUri", EmitDefaultValue=false)]
        public string AuthenticationOauthProfilesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of OAuth Provider objects. Deprecated since 2.25. Replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of OAuth Provider objects. Deprecated since 2.25. Replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="authenticationOauthProvidersUri", EmitDefaultValue=false)]
        public string AuthenticationOauthProvidersUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Authorization Group objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Authorization Group objects.</value>
        [DataMember(Name="authorizationGroupsUri", EmitDefaultValue=false)]
        public string AuthorizationGroupsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Bridge objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Bridge objects.</value>
        [DataMember(Name="bridgesUri", EmitDefaultValue=false)]
        public string BridgesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Certificate Matching Rule objects. Available since 2.27.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Certificate Matching Rule objects. Available since 2.27.</value>
        [DataMember(Name="certMatchingRulesUri", EmitDefaultValue=false)]
        public string CertMatchingRulesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Client Profile objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Client Profile objects.</value>
        [DataMember(Name="clientProfilesUri", EmitDefaultValue=false)]
        public string ClientProfilesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Client Username objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Client Username objects.</value>
        [DataMember(Name="clientUsernamesUri", EmitDefaultValue=false)]
        public string ClientUsernamesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Distributed Cache objects. Available since 2.11.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Distributed Cache objects. Available since 2.11.</value>
        [DataMember(Name="distributedCachesUri", EmitDefaultValue=false)]
        public string DistributedCachesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of DMR Bridge objects. Available since 2.11.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of DMR Bridge objects. Available since 2.11.</value>
        [DataMember(Name="dmrBridgesUri", EmitDefaultValue=false)]
        public string DmrBridgesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of JNDI Connection Factory objects. Available since 2.2.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of JNDI Connection Factory objects. Available since 2.2.</value>
        [DataMember(Name="jndiConnectionFactoriesUri", EmitDefaultValue=false)]
        public string JndiConnectionFactoriesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of JNDI Queue objects. Available since 2.2.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of JNDI Queue objects. Available since 2.2.</value>
        [DataMember(Name="jndiQueuesUri", EmitDefaultValue=false)]
        public string JndiQueuesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of JNDI Topic objects. Available since 2.2.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of JNDI Topic objects. Available since 2.2.</value>
        [DataMember(Name="jndiTopicsUri", EmitDefaultValue=false)]
        public string JndiTopicsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of MQTT Retain Cache objects. Available since 2.11.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of MQTT Retain Cache objects. Available since 2.11.</value>
        [DataMember(Name="mqttRetainCachesUri", EmitDefaultValue=false)]
        public string MqttRetainCachesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of MQTT Session objects. Available since 2.1.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of MQTT Session objects. Available since 2.1.</value>
        [DataMember(Name="mqttSessionsUri", EmitDefaultValue=false)]
        public string MqttSessionsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Queue Template objects. Available since 2.14.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Queue Template objects. Available since 2.14.</value>
        [DataMember(Name="queueTemplatesUri", EmitDefaultValue=false)]
        public string QueueTemplatesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Queue objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Queue objects.</value>
        [DataMember(Name="queuesUri", EmitDefaultValue=false)]
        public string QueuesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Replay Log objects. Available since 2.10.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Replay Log objects. Available since 2.10.</value>
        [DataMember(Name="replayLogsUri", EmitDefaultValue=false)]
        public string ReplayLogsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Replicated Topic objects. Available since 2.1.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Replicated Topic objects. Available since 2.1.</value>
        [DataMember(Name="replicatedTopicsUri", EmitDefaultValue=false)]
        public string ReplicatedTopicsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of REST Delivery Point objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of REST Delivery Point objects.</value>
        [DataMember(Name="restDeliveryPointsUri", EmitDefaultValue=false)]
        public string RestDeliveryPointsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Sequenced Topic objects.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Sequenced Topic objects.</value>
        [DataMember(Name="sequencedTopicsUri", EmitDefaultValue=false)]
        public string SequencedTopicsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Telemetry Profile objects. Available since 2.31.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Telemetry Profile objects. Available since 2.31.</value>
        [DataMember(Name="telemetryProfilesUri", EmitDefaultValue=false)]
        public string TelemetryProfilesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Topic Endpoint Template objects. Available since 2.14.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Topic Endpoint Template objects. Available since 2.14.</value>
        [DataMember(Name="topicEndpointTemplatesUri", EmitDefaultValue=false)]
        public string TopicEndpointTemplatesUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN&#x27;s collection of Topic Endpoint objects. Available since 2.1.
        /// </summary>
        /// <value>The URI of this Message VPN&#x27;s collection of Topic Endpoint objects. Available since 2.1.</value>
        [DataMember(Name="topicEndpointsUri", EmitDefaultValue=false)]
        public string TopicEndpointsUri { get; set; }

        /// <summary>
        /// The URI of this Message VPN object.
        /// </summary>
        /// <value>The URI of this Message VPN object.</value>
        [DataMember(Name="uri", EmitDefaultValue=false)]
        public string Uri { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnLinks {\n");
            sb.Append("  AclProfilesUri: ").Append(AclProfilesUri).Append("\n");
            sb.Append("  AuthenticationOauthProfilesUri: ").Append(AuthenticationOauthProfilesUri).Append("\n");
            sb.Append("  AuthenticationOauthProvidersUri: ").Append(AuthenticationOauthProvidersUri).Append("\n");
            sb.Append("  AuthorizationGroupsUri: ").Append(AuthorizationGroupsUri).Append("\n");
            sb.Append("  BridgesUri: ").Append(BridgesUri).Append("\n");
            sb.Append("  CertMatchingRulesUri: ").Append(CertMatchingRulesUri).Append("\n");
            sb.Append("  ClientProfilesUri: ").Append(ClientProfilesUri).Append("\n");
            sb.Append("  ClientUsernamesUri: ").Append(ClientUsernamesUri).Append("\n");
            sb.Append("  DistributedCachesUri: ").Append(DistributedCachesUri).Append("\n");
            sb.Append("  DmrBridgesUri: ").Append(DmrBridgesUri).Append("\n");
            sb.Append("  JndiConnectionFactoriesUri: ").Append(JndiConnectionFactoriesUri).Append("\n");
            sb.Append("  JndiQueuesUri: ").Append(JndiQueuesUri).Append("\n");
            sb.Append("  JndiTopicsUri: ").Append(JndiTopicsUri).Append("\n");
            sb.Append("  MqttRetainCachesUri: ").Append(MqttRetainCachesUri).Append("\n");
            sb.Append("  MqttSessionsUri: ").Append(MqttSessionsUri).Append("\n");
            sb.Append("  QueueTemplatesUri: ").Append(QueueTemplatesUri).Append("\n");
            sb.Append("  QueuesUri: ").Append(QueuesUri).Append("\n");
            sb.Append("  ReplayLogsUri: ").Append(ReplayLogsUri).Append("\n");
            sb.Append("  ReplicatedTopicsUri: ").Append(ReplicatedTopicsUri).Append("\n");
            sb.Append("  RestDeliveryPointsUri: ").Append(RestDeliveryPointsUri).Append("\n");
            sb.Append("  SequencedTopicsUri: ").Append(SequencedTopicsUri).Append("\n");
            sb.Append("  TelemetryProfilesUri: ").Append(TelemetryProfilesUri).Append("\n");
            sb.Append("  TopicEndpointTemplatesUri: ").Append(TopicEndpointTemplatesUri).Append("\n");
            sb.Append("  TopicEndpointsUri: ").Append(TopicEndpointsUri).Append("\n");
            sb.Append("  Uri: ").Append(Uri).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as MsgVpnLinks);
        }

        /// <summary>
        /// Returns true if MsgVpnLinks instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnLinks to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnLinks input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AclProfilesUri == input.AclProfilesUri ||
                    (this.AclProfilesUri != null &&
                    this.AclProfilesUri.Equals(input.AclProfilesUri))
                ) && 
                (
                    this.AuthenticationOauthProfilesUri == input.AuthenticationOauthProfilesUri ||
                    (this.AuthenticationOauthProfilesUri != null &&
                    this.AuthenticationOauthProfilesUri.Equals(input.AuthenticationOauthProfilesUri))
                ) && 
                (
                    this.AuthenticationOauthProvidersUri == input.AuthenticationOauthProvidersUri ||
                    (this.AuthenticationOauthProvidersUri != null &&
                    this.AuthenticationOauthProvidersUri.Equals(input.AuthenticationOauthProvidersUri))
                ) && 
                (
                    this.AuthorizationGroupsUri == input.AuthorizationGroupsUri ||
                    (this.AuthorizationGroupsUri != null &&
                    this.AuthorizationGroupsUri.Equals(input.AuthorizationGroupsUri))
                ) && 
                (
                    this.BridgesUri == input.BridgesUri ||
                    (this.BridgesUri != null &&
                    this.BridgesUri.Equals(input.BridgesUri))
                ) && 
                (
                    this.CertMatchingRulesUri == input.CertMatchingRulesUri ||
                    (this.CertMatchingRulesUri != null &&
                    this.CertMatchingRulesUri.Equals(input.CertMatchingRulesUri))
                ) && 
                (
                    this.ClientProfilesUri == input.ClientProfilesUri ||
                    (this.ClientProfilesUri != null &&
                    this.ClientProfilesUri.Equals(input.ClientProfilesUri))
                ) && 
                (
                    this.ClientUsernamesUri == input.ClientUsernamesUri ||
                    (this.ClientUsernamesUri != null &&
                    this.ClientUsernamesUri.Equals(input.ClientUsernamesUri))
                ) && 
                (
                    this.DistributedCachesUri == input.DistributedCachesUri ||
                    (this.DistributedCachesUri != null &&
                    this.DistributedCachesUri.Equals(input.DistributedCachesUri))
                ) && 
                (
                    this.DmrBridgesUri == input.DmrBridgesUri ||
                    (this.DmrBridgesUri != null &&
                    this.DmrBridgesUri.Equals(input.DmrBridgesUri))
                ) && 
                (
                    this.JndiConnectionFactoriesUri == input.JndiConnectionFactoriesUri ||
                    (this.JndiConnectionFactoriesUri != null &&
                    this.JndiConnectionFactoriesUri.Equals(input.JndiConnectionFactoriesUri))
                ) && 
                (
                    this.JndiQueuesUri == input.JndiQueuesUri ||
                    (this.JndiQueuesUri != null &&
                    this.JndiQueuesUri.Equals(input.JndiQueuesUri))
                ) && 
                (
                    this.JndiTopicsUri == input.JndiTopicsUri ||
                    (this.JndiTopicsUri != null &&
                    this.JndiTopicsUri.Equals(input.JndiTopicsUri))
                ) && 
                (
                    this.MqttRetainCachesUri == input.MqttRetainCachesUri ||
                    (this.MqttRetainCachesUri != null &&
                    this.MqttRetainCachesUri.Equals(input.MqttRetainCachesUri))
                ) && 
                (
                    this.MqttSessionsUri == input.MqttSessionsUri ||
                    (this.MqttSessionsUri != null &&
                    this.MqttSessionsUri.Equals(input.MqttSessionsUri))
                ) && 
                (
                    this.QueueTemplatesUri == input.QueueTemplatesUri ||
                    (this.QueueTemplatesUri != null &&
                    this.QueueTemplatesUri.Equals(input.QueueTemplatesUri))
                ) && 
                (
                    this.QueuesUri == input.QueuesUri ||
                    (this.QueuesUri != null &&
                    this.QueuesUri.Equals(input.QueuesUri))
                ) && 
                (
                    this.ReplayLogsUri == input.ReplayLogsUri ||
                    (this.ReplayLogsUri != null &&
                    this.ReplayLogsUri.Equals(input.ReplayLogsUri))
                ) && 
                (
                    this.ReplicatedTopicsUri == input.ReplicatedTopicsUri ||
                    (this.ReplicatedTopicsUri != null &&
                    this.ReplicatedTopicsUri.Equals(input.ReplicatedTopicsUri))
                ) && 
                (
                    this.RestDeliveryPointsUri == input.RestDeliveryPointsUri ||
                    (this.RestDeliveryPointsUri != null &&
                    this.RestDeliveryPointsUri.Equals(input.RestDeliveryPointsUri))
                ) && 
                (
                    this.SequencedTopicsUri == input.SequencedTopicsUri ||
                    (this.SequencedTopicsUri != null &&
                    this.SequencedTopicsUri.Equals(input.SequencedTopicsUri))
                ) && 
                (
                    this.TelemetryProfilesUri == input.TelemetryProfilesUri ||
                    (this.TelemetryProfilesUri != null &&
                    this.TelemetryProfilesUri.Equals(input.TelemetryProfilesUri))
                ) && 
                (
                    this.TopicEndpointTemplatesUri == input.TopicEndpointTemplatesUri ||
                    (this.TopicEndpointTemplatesUri != null &&
                    this.TopicEndpointTemplatesUri.Equals(input.TopicEndpointTemplatesUri))
                ) && 
                (
                    this.TopicEndpointsUri == input.TopicEndpointsUri ||
                    (this.TopicEndpointsUri != null &&
                    this.TopicEndpointsUri.Equals(input.TopicEndpointsUri))
                ) && 
                (
                    this.Uri == input.Uri ||
                    (this.Uri != null &&
                    this.Uri.Equals(input.Uri))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AclProfilesUri != null)
                    hashCode = hashCode * 59 + this.AclProfilesUri.GetHashCode();
                if (this.AuthenticationOauthProfilesUri != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthProfilesUri.GetHashCode();
                if (this.AuthenticationOauthProvidersUri != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthProvidersUri.GetHashCode();
                if (this.AuthorizationGroupsUri != null)
                    hashCode = hashCode * 59 + this.AuthorizationGroupsUri.GetHashCode();
                if (this.BridgesUri != null)
                    hashCode = hashCode * 59 + this.BridgesUri.GetHashCode();
                if (this.CertMatchingRulesUri != null)
                    hashCode = hashCode * 59 + this.CertMatchingRulesUri.GetHashCode();
                if (this.ClientProfilesUri != null)
                    hashCode = hashCode * 59 + this.ClientProfilesUri.GetHashCode();
                if (this.ClientUsernamesUri != null)
                    hashCode = hashCode * 59 + this.ClientUsernamesUri.GetHashCode();
                if (this.DistributedCachesUri != null)
                    hashCode = hashCode * 59 + this.DistributedCachesUri.GetHashCode();
                if (this.DmrBridgesUri != null)
                    hashCode = hashCode * 59 + this.DmrBridgesUri.GetHashCode();
                if (this.JndiConnectionFactoriesUri != null)
                    hashCode = hashCode * 59 + this.JndiConnectionFactoriesUri.GetHashCode();
                if (this.JndiQueuesUri != null)
                    hashCode = hashCode * 59 + this.JndiQueuesUri.GetHashCode();
                if (this.JndiTopicsUri != null)
                    hashCode = hashCode * 59 + this.JndiTopicsUri.GetHashCode();
                if (this.MqttRetainCachesUri != null)
                    hashCode = hashCode * 59 + this.MqttRetainCachesUri.GetHashCode();
                if (this.MqttSessionsUri != null)
                    hashCode = hashCode * 59 + this.MqttSessionsUri.GetHashCode();
                if (this.QueueTemplatesUri != null)
                    hashCode = hashCode * 59 + this.QueueTemplatesUri.GetHashCode();
                if (this.QueuesUri != null)
                    hashCode = hashCode * 59 + this.QueuesUri.GetHashCode();
                if (this.ReplayLogsUri != null)
                    hashCode = hashCode * 59 + this.ReplayLogsUri.GetHashCode();
                if (this.ReplicatedTopicsUri != null)
                    hashCode = hashCode * 59 + this.ReplicatedTopicsUri.GetHashCode();
                if (this.RestDeliveryPointsUri != null)
                    hashCode = hashCode * 59 + this.RestDeliveryPointsUri.GetHashCode();
                if (this.SequencedTopicsUri != null)
                    hashCode = hashCode * 59 + this.SequencedTopicsUri.GetHashCode();
                if (this.TelemetryProfilesUri != null)
                    hashCode = hashCode * 59 + this.TelemetryProfilesUri.GetHashCode();
                if (this.TopicEndpointTemplatesUri != null)
                    hashCode = hashCode * 59 + this.TopicEndpointTemplatesUri.GetHashCode();
                if (this.TopicEndpointsUri != null)
                    hashCode = hashCode * 59 + this.TopicEndpointsUri.GetHashCode();
                if (this.Uri != null)
                    hashCode = hashCode * 59 + this.Uri.GetHashCode();
                return hashCode;
            }
        }
    }
}
