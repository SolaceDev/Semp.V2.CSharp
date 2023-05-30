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
    /// MsgVpnBridge
    /// </summary>
    [DataContract]
        public partial class MsgVpnBridge :  IEquatable<MsgVpnBridge>
    {
        /// <summary>
        /// The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum BridgeVirtualRouterEnum
        {
            /// <summary>
            /// Enum Primary for value: primary
            /// </summary>
            [EnumMember(Value = "primary")]
            Primary = 1,
            /// <summary>
            /// Enum Backup for value: backup
            /// </summary>
            [EnumMember(Value = "backup")]
            Backup = 2,
            /// <summary>
            /// Enum Auto for value: auto
            /// </summary>
            [EnumMember(Value = "auto")]
            Auto = 3        }
        /// <summary>
        /// The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; </value>
        [DataMember(Name="bridgeVirtualRouter", EmitDefaultValue=false)]
        public BridgeVirtualRouterEnum? BridgeVirtualRouter { get; set; }
        /// <summary>
        /// The authentication scheme for the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme for the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum RemoteAuthenticationSchemeEnum
        {
            /// <summary>
            /// Enum Basic for value: basic
            /// </summary>
            [EnumMember(Value = "basic")]
            Basic = 1,
            /// <summary>
            /// Enum ClientCertificate for value: client-certificate
            /// </summary>
            [EnumMember(Value = "client-certificate")]
            ClientCertificate = 2        }
        /// <summary>
        /// The authentication scheme for the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme for the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [DataMember(Name="remoteAuthenticationScheme", EmitDefaultValue=false)]
        public RemoteAuthenticationSchemeEnum? RemoteAuthenticationScheme { get; set; }
        /// <summary>
        /// The priority for deliver-to-one (DTO) messages transmitted from the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;p1\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;p1\&quot; - The 1st or highest priority. \&quot;p2\&quot; - The 2nd highest priority. \&quot;p3\&quot; - The 3rd highest priority. \&quot;p4\&quot; - The 4th highest priority. \&quot;da\&quot; - Ignore priority and deliver always. &lt;/pre&gt; 
        /// </summary>
        /// <value>The priority for deliver-to-one (DTO) messages transmitted from the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;p1\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;p1\&quot; - The 1st or highest priority. \&quot;p2\&quot; - The 2nd highest priority. \&quot;p3\&quot; - The 3rd highest priority. \&quot;p4\&quot; - The 4th highest priority. \&quot;da\&quot; - Ignore priority and deliver always. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum RemoteDeliverToOnePriorityEnum
        {
            /// <summary>
            /// Enum P1 for value: p1
            /// </summary>
            [EnumMember(Value = "p1")]
            P1 = 1,
            /// <summary>
            /// Enum P2 for value: p2
            /// </summary>
            [EnumMember(Value = "p2")]
            P2 = 2,
            /// <summary>
            /// Enum P3 for value: p3
            /// </summary>
            [EnumMember(Value = "p3")]
            P3 = 3,
            /// <summary>
            /// Enum P4 for value: p4
            /// </summary>
            [EnumMember(Value = "p4")]
            P4 = 4,
            /// <summary>
            /// Enum Da for value: da
            /// </summary>
            [EnumMember(Value = "da")]
            Da = 5        }
        /// <summary>
        /// The priority for deliver-to-one (DTO) messages transmitted from the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;p1\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;p1\&quot; - The 1st or highest priority. \&quot;p2\&quot; - The 2nd highest priority. \&quot;p3\&quot; - The 3rd highest priority. \&quot;p4\&quot; - The 4th highest priority. \&quot;da\&quot; - Ignore priority and deliver always. &lt;/pre&gt; 
        /// </summary>
        /// <value>The priority for deliver-to-one (DTO) messages transmitted from the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;p1\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;p1\&quot; - The 1st or highest priority. \&quot;p2\&quot; - The 2nd highest priority. \&quot;p3\&quot; - The 3rd highest priority. \&quot;p4\&quot; - The 4th highest priority. \&quot;da\&quot; - Ignore priority and deliver always. &lt;/pre&gt; </value>
        [DataMember(Name="remoteDeliverToOnePriority", EmitDefaultValue=false)]
        public RemoteDeliverToOnePriorityEnum? RemoteDeliverToOnePriority { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnBridge" /> class.
        /// </summary>
        /// <param name="bridgeName">The name of the Bridge..</param>
        /// <param name="bridgeVirtualRouter">The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; .</param>
        /// <param name="enabled">Enable or disable the Bridge. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="maxTtl">The maximum time-to-live (TTL) in hops. Messages are discarded if their TTL exceeds this value. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8&#x60;..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="remoteAuthenticationBasicClientUsername">The Client Username the Bridge uses to login to the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="remoteAuthenticationBasicPassword">The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="remoteAuthenticationClientCertContent">The PEM formatted content for the client certificate used by the Bridge to login to the remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="remoteAuthenticationClientCertPassword">The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="remoteAuthenticationScheme">The authentication scheme for the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; .</param>
        /// <param name="remoteConnectionRetryCount">The maximum number of retry attempts to establish a connection to the remote Message VPN. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="remoteConnectionRetryDelay">The number of seconds the broker waits for the bridge connection to be established before attempting a new connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="remoteDeliverToOnePriority">The priority for deliver-to-one (DTO) messages transmitted from the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;p1\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;p1\&quot; - The 1st or highest priority. \&quot;p2\&quot; - The 2nd highest priority. \&quot;p3\&quot; - The 3rd highest priority. \&quot;p4\&quot; - The 4th highest priority. \&quot;da\&quot; - Ignore priority and deliver always. &lt;/pre&gt; .</param>
        /// <param name="tlsCipherSuiteList">The colon-separated list of cipher suites supported for TLS connections to the remote Message VPN. The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        public MsgVpnBridge(string bridgeName = default(string), BridgeVirtualRouterEnum? bridgeVirtualRouter = default(BridgeVirtualRouterEnum?), bool? enabled = default(bool?), long? maxTtl = default(long?), string msgVpnName = default(string), string remoteAuthenticationBasicClientUsername = default(string), string remoteAuthenticationBasicPassword = default(string), string remoteAuthenticationClientCertContent = default(string), string remoteAuthenticationClientCertPassword = default(string), RemoteAuthenticationSchemeEnum? remoteAuthenticationScheme = default(RemoteAuthenticationSchemeEnum?), long? remoteConnectionRetryCount = default(long?), long? remoteConnectionRetryDelay = default(long?), RemoteDeliverToOnePriorityEnum? remoteDeliverToOnePriority = default(RemoteDeliverToOnePriorityEnum?), string tlsCipherSuiteList = default(string))
        {
            this.BridgeName = bridgeName;
            this.BridgeVirtualRouter = bridgeVirtualRouter;
            this.Enabled = enabled;
            this.MaxTtl = maxTtl;
            this.MsgVpnName = msgVpnName;
            this.RemoteAuthenticationBasicClientUsername = remoteAuthenticationBasicClientUsername;
            this.RemoteAuthenticationBasicPassword = remoteAuthenticationBasicPassword;
            this.RemoteAuthenticationClientCertContent = remoteAuthenticationClientCertContent;
            this.RemoteAuthenticationClientCertPassword = remoteAuthenticationClientCertPassword;
            this.RemoteAuthenticationScheme = remoteAuthenticationScheme;
            this.RemoteConnectionRetryCount = remoteConnectionRetryCount;
            this.RemoteConnectionRetryDelay = remoteConnectionRetryDelay;
            this.RemoteDeliverToOnePriority = remoteDeliverToOnePriority;
            this.TlsCipherSuiteList = tlsCipherSuiteList;
        }
        
        /// <summary>
        /// The name of the Bridge.
        /// </summary>
        /// <value>The name of the Bridge.</value>
        [DataMember(Name="bridgeName", EmitDefaultValue=false)]
        public string BridgeName { get; set; }


        /// <summary>
        /// Enable or disable the Bridge. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the Bridge. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// The maximum time-to-live (TTL) in hops. Messages are discarded if their TTL exceeds this value. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8&#x60;.
        /// </summary>
        /// <value>The maximum time-to-live (TTL) in hops. Messages are discarded if their TTL exceeds this value. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8&#x60;.</value>
        [DataMember(Name="maxTtl", EmitDefaultValue=false)]
        public long? MaxTtl { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The Client Username the Bridge uses to login to the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The Client Username the Bridge uses to login to the remote Message VPN. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="remoteAuthenticationBasicClientUsername", EmitDefaultValue=false)]
        public string RemoteAuthenticationBasicClientUsername { get; set; }

        /// <summary>
        /// The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="remoteAuthenticationBasicPassword", EmitDefaultValue=false)]
        public string RemoteAuthenticationBasicPassword { get; set; }

        /// <summary>
        /// The PEM formatted content for the client certificate used by the Bridge to login to the remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The PEM formatted content for the client certificate used by the Bridge to login to the remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="remoteAuthenticationClientCertContent", EmitDefaultValue=false)]
        public string RemoteAuthenticationClientCertContent { get; set; }

        /// <summary>
        /// The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="remoteAuthenticationClientCertPassword", EmitDefaultValue=false)]
        public string RemoteAuthenticationClientCertPassword { get; set; }


        /// <summary>
        /// The maximum number of retry attempts to establish a connection to the remote Message VPN. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum number of retry attempts to establish a connection to the remote Message VPN. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="remoteConnectionRetryCount", EmitDefaultValue=false)]
        public long? RemoteConnectionRetryCount { get; set; }

        /// <summary>
        /// The number of seconds the broker waits for the bridge connection to be established before attempting a new connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The number of seconds the broker waits for the bridge connection to be established before attempting a new connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="remoteConnectionRetryDelay", EmitDefaultValue=false)]
        public long? RemoteConnectionRetryDelay { get; set; }


        /// <summary>
        /// The colon-separated list of cipher suites supported for TLS connections to the remote Message VPN. The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The colon-separated list of cipher suites supported for TLS connections to the remote Message VPN. The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="tlsCipherSuiteList", EmitDefaultValue=false)]
        public string TlsCipherSuiteList { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnBridge {\n");
            sb.Append("  BridgeName: ").Append(BridgeName).Append("\n");
            sb.Append("  BridgeVirtualRouter: ").Append(BridgeVirtualRouter).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  MaxTtl: ").Append(MaxTtl).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  RemoteAuthenticationBasicClientUsername: ").Append(RemoteAuthenticationBasicClientUsername).Append("\n");
            sb.Append("  RemoteAuthenticationBasicPassword: ").Append(RemoteAuthenticationBasicPassword).Append("\n");
            sb.Append("  RemoteAuthenticationClientCertContent: ").Append(RemoteAuthenticationClientCertContent).Append("\n");
            sb.Append("  RemoteAuthenticationClientCertPassword: ").Append(RemoteAuthenticationClientCertPassword).Append("\n");
            sb.Append("  RemoteAuthenticationScheme: ").Append(RemoteAuthenticationScheme).Append("\n");
            sb.Append("  RemoteConnectionRetryCount: ").Append(RemoteConnectionRetryCount).Append("\n");
            sb.Append("  RemoteConnectionRetryDelay: ").Append(RemoteConnectionRetryDelay).Append("\n");
            sb.Append("  RemoteDeliverToOnePriority: ").Append(RemoteDeliverToOnePriority).Append("\n");
            sb.Append("  TlsCipherSuiteList: ").Append(TlsCipherSuiteList).Append("\n");
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
            return this.Equals(input as MsgVpnBridge);
        }

        /// <summary>
        /// Returns true if MsgVpnBridge instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnBridge to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnBridge input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.BridgeName == input.BridgeName ||
                    (this.BridgeName != null &&
                    this.BridgeName.Equals(input.BridgeName))
                ) && 
                (
                    this.BridgeVirtualRouter == input.BridgeVirtualRouter ||
                    (this.BridgeVirtualRouter != null &&
                    this.BridgeVirtualRouter.Equals(input.BridgeVirtualRouter))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.MaxTtl == input.MaxTtl ||
                    (this.MaxTtl != null &&
                    this.MaxTtl.Equals(input.MaxTtl))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.RemoteAuthenticationBasicClientUsername == input.RemoteAuthenticationBasicClientUsername ||
                    (this.RemoteAuthenticationBasicClientUsername != null &&
                    this.RemoteAuthenticationBasicClientUsername.Equals(input.RemoteAuthenticationBasicClientUsername))
                ) && 
                (
                    this.RemoteAuthenticationBasicPassword == input.RemoteAuthenticationBasicPassword ||
                    (this.RemoteAuthenticationBasicPassword != null &&
                    this.RemoteAuthenticationBasicPassword.Equals(input.RemoteAuthenticationBasicPassword))
                ) && 
                (
                    this.RemoteAuthenticationClientCertContent == input.RemoteAuthenticationClientCertContent ||
                    (this.RemoteAuthenticationClientCertContent != null &&
                    this.RemoteAuthenticationClientCertContent.Equals(input.RemoteAuthenticationClientCertContent))
                ) && 
                (
                    this.RemoteAuthenticationClientCertPassword == input.RemoteAuthenticationClientCertPassword ||
                    (this.RemoteAuthenticationClientCertPassword != null &&
                    this.RemoteAuthenticationClientCertPassword.Equals(input.RemoteAuthenticationClientCertPassword))
                ) && 
                (
                    this.RemoteAuthenticationScheme == input.RemoteAuthenticationScheme ||
                    (this.RemoteAuthenticationScheme != null &&
                    this.RemoteAuthenticationScheme.Equals(input.RemoteAuthenticationScheme))
                ) && 
                (
                    this.RemoteConnectionRetryCount == input.RemoteConnectionRetryCount ||
                    (this.RemoteConnectionRetryCount != null &&
                    this.RemoteConnectionRetryCount.Equals(input.RemoteConnectionRetryCount))
                ) && 
                (
                    this.RemoteConnectionRetryDelay == input.RemoteConnectionRetryDelay ||
                    (this.RemoteConnectionRetryDelay != null &&
                    this.RemoteConnectionRetryDelay.Equals(input.RemoteConnectionRetryDelay))
                ) && 
                (
                    this.RemoteDeliverToOnePriority == input.RemoteDeliverToOnePriority ||
                    (this.RemoteDeliverToOnePriority != null &&
                    this.RemoteDeliverToOnePriority.Equals(input.RemoteDeliverToOnePriority))
                ) && 
                (
                    this.TlsCipherSuiteList == input.TlsCipherSuiteList ||
                    (this.TlsCipherSuiteList != null &&
                    this.TlsCipherSuiteList.Equals(input.TlsCipherSuiteList))
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
                if (this.BridgeName != null)
                    hashCode = hashCode * 59 + this.BridgeName.GetHashCode();
                if (this.BridgeVirtualRouter != null)
                    hashCode = hashCode * 59 + this.BridgeVirtualRouter.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.MaxTtl != null)
                    hashCode = hashCode * 59 + this.MaxTtl.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.RemoteAuthenticationBasicClientUsername != null)
                    hashCode = hashCode * 59 + this.RemoteAuthenticationBasicClientUsername.GetHashCode();
                if (this.RemoteAuthenticationBasicPassword != null)
                    hashCode = hashCode * 59 + this.RemoteAuthenticationBasicPassword.GetHashCode();
                if (this.RemoteAuthenticationClientCertContent != null)
                    hashCode = hashCode * 59 + this.RemoteAuthenticationClientCertContent.GetHashCode();
                if (this.RemoteAuthenticationClientCertPassword != null)
                    hashCode = hashCode * 59 + this.RemoteAuthenticationClientCertPassword.GetHashCode();
                if (this.RemoteAuthenticationScheme != null)
                    hashCode = hashCode * 59 + this.RemoteAuthenticationScheme.GetHashCode();
                if (this.RemoteConnectionRetryCount != null)
                    hashCode = hashCode * 59 + this.RemoteConnectionRetryCount.GetHashCode();
                if (this.RemoteConnectionRetryDelay != null)
                    hashCode = hashCode * 59 + this.RemoteConnectionRetryDelay.GetHashCode();
                if (this.RemoteDeliverToOnePriority != null)
                    hashCode = hashCode * 59 + this.RemoteDeliverToOnePriority.GetHashCode();
                if (this.TlsCipherSuiteList != null)
                    hashCode = hashCode * 59 + this.TlsCipherSuiteList.GetHashCode();
                return hashCode;
            }
        }
    }
}
