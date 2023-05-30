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
    /// OauthProfile
    /// </summary>
    [DataContract]
        public partial class OauthProfile :  IEquatable<OauthProfile>
    {
        /// <summary>
        /// The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32.
        /// </summary>
        /// <value>The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AccessLevelGroupsClaimStringFormatEnum
        {
            /// <summary>
            /// Enum Single for value: single
            /// </summary>
            [EnumMember(Value = "single")]
            Single = 1,
            /// <summary>
            /// Enum SpaceDelimited for value: space-delimited
            /// </summary>
            [EnumMember(Value = "space-delimited")]
            SpaceDelimited = 2        }
        /// <summary>
        /// The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32.
        /// </summary>
        /// <value>The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32.</value>
        [DataMember(Name="accessLevelGroupsClaimStringFormat", EmitDefaultValue=false)]
        public AccessLevelGroupsClaimStringFormatEnum? AccessLevelGroupsClaimStringFormat { get; set; }
        /// <summary>
        /// The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum DefaultGlobalAccessLevelEnum
        {
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            /// <summary>
            /// Enum ReadOnly for value: read-only
            /// </summary>
            [EnumMember(Value = "read-only")]
            ReadOnly = 2,
            /// <summary>
            /// Enum ReadWrite for value: read-write
            /// </summary>
            [EnumMember(Value = "read-write")]
            ReadWrite = 3,
            /// <summary>
            /// Enum Admin for value: admin
            /// </summary>
            [EnumMember(Value = "admin")]
            Admin = 4        }
        /// <summary>
        /// The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt; </value>
        [DataMember(Name="defaultGlobalAccessLevel", EmitDefaultValue=false)]
        public DefaultGlobalAccessLevelEnum? DefaultGlobalAccessLevel { get; set; }
        /// <summary>
        /// The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum DefaultMsgVpnAccessLevelEnum
        {
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            /// <summary>
            /// Enum ReadOnly for value: read-only
            /// </summary>
            [EnumMember(Value = "read-only")]
            ReadOnly = 2,
            /// <summary>
            /// Enum ReadWrite for value: read-write
            /// </summary>
            [EnumMember(Value = "read-write")]
            ReadWrite = 3        }
        /// <summary>
        /// The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt; </value>
        [DataMember(Name="defaultMsgVpnAccessLevel", EmitDefaultValue=false)]
        public DefaultMsgVpnAccessLevelEnum? DefaultMsgVpnAccessLevel { get; set; }
        /// <summary>
        /// The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum OauthRoleEnum
        {
            /// <summary>
            /// Enum Client for value: client
            /// </summary>
            [EnumMember(Value = "client")]
            Client = 1,
            /// <summary>
            /// Enum ResourceServer for value: resource-server
            /// </summary>
            [EnumMember(Value = "resource-server")]
            ResourceServer = 2        }
        /// <summary>
        /// The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt; </value>
        [DataMember(Name="oauthRole", EmitDefaultValue=false)]
        public OauthRoleEnum? OauthRole { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="OauthProfile" /> class.
        /// </summary>
        /// <param name="accessLevelGroupsClaimName">The name of the groups claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;groups\&quot;&#x60;..</param>
        /// <param name="accessLevelGroupsClaimStringFormat">The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32..</param>
        /// <param name="clientId">The OAuth client id. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="clientRedirectUri">The OAuth redirect URI. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="clientRequiredType">The required value for the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;JWT\&quot;&#x60;..</param>
        /// <param name="clientScope">The OAuth scope. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;openid email\&quot;&#x60;..</param>
        /// <param name="clientSecret">The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="clientValidateTypeEnabled">Enable or disable verification of the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="defaultGlobalAccessLevel">The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt; .</param>
        /// <param name="defaultMsgVpnAccessLevel">The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt; .</param>
        /// <param name="displayName">The user friendly name for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="enabled">Enable or disable the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="endpointAuthorization">The OAuth authorization endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointDiscovery">The OpenID Connect discovery endpoint or OAuth Authorization Server Metadata endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointDiscoveryRefreshInterval">The number of seconds between discovery endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;..</param>
        /// <param name="endpointIntrospection">The OAuth introspection endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointIntrospectionTimeout">The maximum time in seconds a token introspection request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="endpointJwks">The OAuth JWKS endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointJwksRefreshInterval">The number of seconds between JWKS endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;..</param>
        /// <param name="endpointToken">The OAuth token endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointTokenTimeout">The maximum time in seconds a token request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="endpointUserinfo">The OpenID Connect Userinfo endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="endpointUserinfoTimeout">The maximum time in seconds a userinfo request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="interactiveEnabled">Enable or disable interactive logins via this OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="interactivePromptForExpiredSession">The value of the prompt parameter provided to the OAuth authorization server for login requests where the session has expired. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="interactivePromptForNewSession">The value of the prompt parameter provided to the OAuth authorization server for login requests where the session is new or the user has explicitly logged out. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;select_account\&quot;&#x60;..</param>
        /// <param name="issuer">The Issuer Identifier for the OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="oauthProfileName">The name of the OAuth profile..</param>
        /// <param name="oauthRole">The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt; .</param>
        /// <param name="resourceServerParseAccessTokenEnabled">Enable or disable parsing of the access token as a JWT. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="resourceServerRequiredAudience">The required audience value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="resourceServerRequiredIssuer">The required issuer value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="resourceServerRequiredScope">A space-separated list of scopes that must be present in the scope claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="resourceServerRequiredType">The required TYP value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;at+jwt\&quot;&#x60;..</param>
        /// <param name="resourceServerValidateAudienceEnabled">Enable or disable verification of the audience claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="resourceServerValidateIssuerEnabled">Enable or disable verification of the issuer claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="resourceServerValidateScopeEnabled">Enable or disable verification of the scope claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="resourceServerValidateTypeEnabled">Enable or disable verification of the TYP field in the access token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="sempEnabled">Enable or disable authentication of SEMP requests with OAuth tokens. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="usernameClaimName">The name of the username claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;..</param>
        public OauthProfile(string accessLevelGroupsClaimName = default(string), AccessLevelGroupsClaimStringFormatEnum? accessLevelGroupsClaimStringFormat = default(AccessLevelGroupsClaimStringFormatEnum?), string clientId = default(string), string clientRedirectUri = default(string), string clientRequiredType = default(string), string clientScope = default(string), string clientSecret = default(string), bool? clientValidateTypeEnabled = default(bool?), DefaultGlobalAccessLevelEnum? defaultGlobalAccessLevel = default(DefaultGlobalAccessLevelEnum?), DefaultMsgVpnAccessLevelEnum? defaultMsgVpnAccessLevel = default(DefaultMsgVpnAccessLevelEnum?), string displayName = default(string), bool? enabled = default(bool?), string endpointAuthorization = default(string), string endpointDiscovery = default(string), int? endpointDiscoveryRefreshInterval = default(int?), string endpointIntrospection = default(string), int? endpointIntrospectionTimeout = default(int?), string endpointJwks = default(string), int? endpointJwksRefreshInterval = default(int?), string endpointToken = default(string), int? endpointTokenTimeout = default(int?), string endpointUserinfo = default(string), int? endpointUserinfoTimeout = default(int?), bool? interactiveEnabled = default(bool?), string interactivePromptForExpiredSession = default(string), string interactivePromptForNewSession = default(string), string issuer = default(string), string oauthProfileName = default(string), OauthRoleEnum? oauthRole = default(OauthRoleEnum?), bool? resourceServerParseAccessTokenEnabled = default(bool?), string resourceServerRequiredAudience = default(string), string resourceServerRequiredIssuer = default(string), string resourceServerRequiredScope = default(string), string resourceServerRequiredType = default(string), bool? resourceServerValidateAudienceEnabled = default(bool?), bool? resourceServerValidateIssuerEnabled = default(bool?), bool? resourceServerValidateScopeEnabled = default(bool?), bool? resourceServerValidateTypeEnabled = default(bool?), bool? sempEnabled = default(bool?), string usernameClaimName = default(string))
        {
            this.AccessLevelGroupsClaimName = accessLevelGroupsClaimName;
            this.AccessLevelGroupsClaimStringFormat = accessLevelGroupsClaimStringFormat;
            this.ClientId = clientId;
            this.ClientRedirectUri = clientRedirectUri;
            this.ClientRequiredType = clientRequiredType;
            this.ClientScope = clientScope;
            this.ClientSecret = clientSecret;
            this.ClientValidateTypeEnabled = clientValidateTypeEnabled;
            this.DefaultGlobalAccessLevel = defaultGlobalAccessLevel;
            this.DefaultMsgVpnAccessLevel = defaultMsgVpnAccessLevel;
            this.DisplayName = displayName;
            this.Enabled = enabled;
            this.EndpointAuthorization = endpointAuthorization;
            this.EndpointDiscovery = endpointDiscovery;
            this.EndpointDiscoveryRefreshInterval = endpointDiscoveryRefreshInterval;
            this.EndpointIntrospection = endpointIntrospection;
            this.EndpointIntrospectionTimeout = endpointIntrospectionTimeout;
            this.EndpointJwks = endpointJwks;
            this.EndpointJwksRefreshInterval = endpointJwksRefreshInterval;
            this.EndpointToken = endpointToken;
            this.EndpointTokenTimeout = endpointTokenTimeout;
            this.EndpointUserinfo = endpointUserinfo;
            this.EndpointUserinfoTimeout = endpointUserinfoTimeout;
            this.InteractiveEnabled = interactiveEnabled;
            this.InteractivePromptForExpiredSession = interactivePromptForExpiredSession;
            this.InteractivePromptForNewSession = interactivePromptForNewSession;
            this.Issuer = issuer;
            this.OauthProfileName = oauthProfileName;
            this.OauthRole = oauthRole;
            this.ResourceServerParseAccessTokenEnabled = resourceServerParseAccessTokenEnabled;
            this.ResourceServerRequiredAudience = resourceServerRequiredAudience;
            this.ResourceServerRequiredIssuer = resourceServerRequiredIssuer;
            this.ResourceServerRequiredScope = resourceServerRequiredScope;
            this.ResourceServerRequiredType = resourceServerRequiredType;
            this.ResourceServerValidateAudienceEnabled = resourceServerValidateAudienceEnabled;
            this.ResourceServerValidateIssuerEnabled = resourceServerValidateIssuerEnabled;
            this.ResourceServerValidateScopeEnabled = resourceServerValidateScopeEnabled;
            this.ResourceServerValidateTypeEnabled = resourceServerValidateTypeEnabled;
            this.SempEnabled = sempEnabled;
            this.UsernameClaimName = usernameClaimName;
        }
        
        /// <summary>
        /// The name of the groups claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;groups\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the groups claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;groups\&quot;&#x60;.</value>
        [DataMember(Name="accessLevelGroupsClaimName", EmitDefaultValue=false)]
        public string AccessLevelGroupsClaimName { get; set; }


        /// <summary>
        /// The OAuth client id. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth client id. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="clientId", EmitDefaultValue=false)]
        public string ClientId { get; set; }

        /// <summary>
        /// The OAuth redirect URI. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth redirect URI. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="clientRedirectUri", EmitDefaultValue=false)]
        public string ClientRedirectUri { get; set; }

        /// <summary>
        /// The required value for the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;JWT\&quot;&#x60;.
        /// </summary>
        /// <value>The required value for the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;JWT\&quot;&#x60;.</value>
        [DataMember(Name="clientRequiredType", EmitDefaultValue=false)]
        public string ClientRequiredType { get; set; }

        /// <summary>
        /// The OAuth scope. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;openid email\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth scope. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;openid email\&quot;&#x60;.</value>
        [DataMember(Name="clientScope", EmitDefaultValue=false)]
        public string ClientScope { get; set; }

        /// <summary>
        /// The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="clientSecret", EmitDefaultValue=false)]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Enable or disable verification of the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable verification of the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="clientValidateTypeEnabled", EmitDefaultValue=false)]
        public bool? ClientValidateTypeEnabled { get; set; }



        /// <summary>
        /// The user friendly name for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The user friendly name for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="displayName", EmitDefaultValue=false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Enable or disable the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// The OAuth authorization endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth authorization endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointAuthorization", EmitDefaultValue=false)]
        public string EndpointAuthorization { get; set; }

        /// <summary>
        /// The OpenID Connect discovery endpoint or OAuth Authorization Server Metadata endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OpenID Connect discovery endpoint or OAuth Authorization Server Metadata endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointDiscovery", EmitDefaultValue=false)]
        public string EndpointDiscovery { get; set; }

        /// <summary>
        /// The number of seconds between discovery endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.
        /// </summary>
        /// <value>The number of seconds between discovery endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.</value>
        [DataMember(Name="endpointDiscoveryRefreshInterval", EmitDefaultValue=false)]
        public int? EndpointDiscoveryRefreshInterval { get; set; }

        /// <summary>
        /// The OAuth introspection endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth introspection endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointIntrospection", EmitDefaultValue=false)]
        public string EndpointIntrospection { get; set; }

        /// <summary>
        /// The maximum time in seconds a token introspection request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The maximum time in seconds a token introspection request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="endpointIntrospectionTimeout", EmitDefaultValue=false)]
        public int? EndpointIntrospectionTimeout { get; set; }

        /// <summary>
        /// The OAuth JWKS endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth JWKS endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointJwks", EmitDefaultValue=false)]
        public string EndpointJwks { get; set; }

        /// <summary>
        /// The number of seconds between JWKS endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.
        /// </summary>
        /// <value>The number of seconds between JWKS endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.</value>
        [DataMember(Name="endpointJwksRefreshInterval", EmitDefaultValue=false)]
        public int? EndpointJwksRefreshInterval { get; set; }

        /// <summary>
        /// The OAuth token endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OAuth token endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointToken", EmitDefaultValue=false)]
        public string EndpointToken { get; set; }

        /// <summary>
        /// The maximum time in seconds a token request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The maximum time in seconds a token request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="endpointTokenTimeout", EmitDefaultValue=false)]
        public int? EndpointTokenTimeout { get; set; }

        /// <summary>
        /// The OpenID Connect Userinfo endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The OpenID Connect Userinfo endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="endpointUserinfo", EmitDefaultValue=false)]
        public string EndpointUserinfo { get; set; }

        /// <summary>
        /// The maximum time in seconds a userinfo request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The maximum time in seconds a userinfo request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="endpointUserinfoTimeout", EmitDefaultValue=false)]
        public int? EndpointUserinfoTimeout { get; set; }

        /// <summary>
        /// Enable or disable interactive logins via this OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable interactive logins via this OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="interactiveEnabled", EmitDefaultValue=false)]
        public bool? InteractiveEnabled { get; set; }

        /// <summary>
        /// The value of the prompt parameter provided to the OAuth authorization server for login requests where the session has expired. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The value of the prompt parameter provided to the OAuth authorization server for login requests where the session has expired. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="interactivePromptForExpiredSession", EmitDefaultValue=false)]
        public string InteractivePromptForExpiredSession { get; set; }

        /// <summary>
        /// The value of the prompt parameter provided to the OAuth authorization server for login requests where the session is new or the user has explicitly logged out. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;select_account\&quot;&#x60;.
        /// </summary>
        /// <value>The value of the prompt parameter provided to the OAuth authorization server for login requests where the session is new or the user has explicitly logged out. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;select_account\&quot;&#x60;.</value>
        [DataMember(Name="interactivePromptForNewSession", EmitDefaultValue=false)]
        public string InteractivePromptForNewSession { get; set; }

        /// <summary>
        /// The Issuer Identifier for the OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The Issuer Identifier for the OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="issuer", EmitDefaultValue=false)]
        public string Issuer { get; set; }

        /// <summary>
        /// The name of the OAuth profile.
        /// </summary>
        /// <value>The name of the OAuth profile.</value>
        [DataMember(Name="oauthProfileName", EmitDefaultValue=false)]
        public string OauthProfileName { get; set; }


        /// <summary>
        /// Enable or disable parsing of the access token as a JWT. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable parsing of the access token as a JWT. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="resourceServerParseAccessTokenEnabled", EmitDefaultValue=false)]
        public bool? ResourceServerParseAccessTokenEnabled { get; set; }

        /// <summary>
        /// The required audience value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The required audience value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="resourceServerRequiredAudience", EmitDefaultValue=false)]
        public string ResourceServerRequiredAudience { get; set; }

        /// <summary>
        /// The required issuer value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The required issuer value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="resourceServerRequiredIssuer", EmitDefaultValue=false)]
        public string ResourceServerRequiredIssuer { get; set; }

        /// <summary>
        /// A space-separated list of scopes that must be present in the scope claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>A space-separated list of scopes that must be present in the scope claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="resourceServerRequiredScope", EmitDefaultValue=false)]
        public string ResourceServerRequiredScope { get; set; }

        /// <summary>
        /// The required TYP value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;at+jwt\&quot;&#x60;.
        /// </summary>
        /// <value>The required TYP value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;at+jwt\&quot;&#x60;.</value>
        [DataMember(Name="resourceServerRequiredType", EmitDefaultValue=false)]
        public string ResourceServerRequiredType { get; set; }

        /// <summary>
        /// Enable or disable verification of the audience claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable verification of the audience claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="resourceServerValidateAudienceEnabled", EmitDefaultValue=false)]
        public bool? ResourceServerValidateAudienceEnabled { get; set; }

        /// <summary>
        /// Enable or disable verification of the issuer claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable verification of the issuer claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="resourceServerValidateIssuerEnabled", EmitDefaultValue=false)]
        public bool? ResourceServerValidateIssuerEnabled { get; set; }

        /// <summary>
        /// Enable or disable verification of the scope claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable verification of the scope claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="resourceServerValidateScopeEnabled", EmitDefaultValue=false)]
        public bool? ResourceServerValidateScopeEnabled { get; set; }

        /// <summary>
        /// Enable or disable verification of the TYP field in the access token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable verification of the TYP field in the access token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="resourceServerValidateTypeEnabled", EmitDefaultValue=false)]
        public bool? ResourceServerValidateTypeEnabled { get; set; }

        /// <summary>
        /// Enable or disable authentication of SEMP requests with OAuth tokens. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable authentication of SEMP requests with OAuth tokens. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="sempEnabled", EmitDefaultValue=false)]
        public bool? SempEnabled { get; set; }

        /// <summary>
        /// The name of the username claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the username claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;.</value>
        [DataMember(Name="usernameClaimName", EmitDefaultValue=false)]
        public string UsernameClaimName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OauthProfile {\n");
            sb.Append("  AccessLevelGroupsClaimName: ").Append(AccessLevelGroupsClaimName).Append("\n");
            sb.Append("  AccessLevelGroupsClaimStringFormat: ").Append(AccessLevelGroupsClaimStringFormat).Append("\n");
            sb.Append("  ClientId: ").Append(ClientId).Append("\n");
            sb.Append("  ClientRedirectUri: ").Append(ClientRedirectUri).Append("\n");
            sb.Append("  ClientRequiredType: ").Append(ClientRequiredType).Append("\n");
            sb.Append("  ClientScope: ").Append(ClientScope).Append("\n");
            sb.Append("  ClientSecret: ").Append(ClientSecret).Append("\n");
            sb.Append("  ClientValidateTypeEnabled: ").Append(ClientValidateTypeEnabled).Append("\n");
            sb.Append("  DefaultGlobalAccessLevel: ").Append(DefaultGlobalAccessLevel).Append("\n");
            sb.Append("  DefaultMsgVpnAccessLevel: ").Append(DefaultMsgVpnAccessLevel).Append("\n");
            sb.Append("  DisplayName: ").Append(DisplayName).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  EndpointAuthorization: ").Append(EndpointAuthorization).Append("\n");
            sb.Append("  EndpointDiscovery: ").Append(EndpointDiscovery).Append("\n");
            sb.Append("  EndpointDiscoveryRefreshInterval: ").Append(EndpointDiscoveryRefreshInterval).Append("\n");
            sb.Append("  EndpointIntrospection: ").Append(EndpointIntrospection).Append("\n");
            sb.Append("  EndpointIntrospectionTimeout: ").Append(EndpointIntrospectionTimeout).Append("\n");
            sb.Append("  EndpointJwks: ").Append(EndpointJwks).Append("\n");
            sb.Append("  EndpointJwksRefreshInterval: ").Append(EndpointJwksRefreshInterval).Append("\n");
            sb.Append("  EndpointToken: ").Append(EndpointToken).Append("\n");
            sb.Append("  EndpointTokenTimeout: ").Append(EndpointTokenTimeout).Append("\n");
            sb.Append("  EndpointUserinfo: ").Append(EndpointUserinfo).Append("\n");
            sb.Append("  EndpointUserinfoTimeout: ").Append(EndpointUserinfoTimeout).Append("\n");
            sb.Append("  InteractiveEnabled: ").Append(InteractiveEnabled).Append("\n");
            sb.Append("  InteractivePromptForExpiredSession: ").Append(InteractivePromptForExpiredSession).Append("\n");
            sb.Append("  InteractivePromptForNewSession: ").Append(InteractivePromptForNewSession).Append("\n");
            sb.Append("  Issuer: ").Append(Issuer).Append("\n");
            sb.Append("  OauthProfileName: ").Append(OauthProfileName).Append("\n");
            sb.Append("  OauthRole: ").Append(OauthRole).Append("\n");
            sb.Append("  ResourceServerParseAccessTokenEnabled: ").Append(ResourceServerParseAccessTokenEnabled).Append("\n");
            sb.Append("  ResourceServerRequiredAudience: ").Append(ResourceServerRequiredAudience).Append("\n");
            sb.Append("  ResourceServerRequiredIssuer: ").Append(ResourceServerRequiredIssuer).Append("\n");
            sb.Append("  ResourceServerRequiredScope: ").Append(ResourceServerRequiredScope).Append("\n");
            sb.Append("  ResourceServerRequiredType: ").Append(ResourceServerRequiredType).Append("\n");
            sb.Append("  ResourceServerValidateAudienceEnabled: ").Append(ResourceServerValidateAudienceEnabled).Append("\n");
            sb.Append("  ResourceServerValidateIssuerEnabled: ").Append(ResourceServerValidateIssuerEnabled).Append("\n");
            sb.Append("  ResourceServerValidateScopeEnabled: ").Append(ResourceServerValidateScopeEnabled).Append("\n");
            sb.Append("  ResourceServerValidateTypeEnabled: ").Append(ResourceServerValidateTypeEnabled).Append("\n");
            sb.Append("  SempEnabled: ").Append(SempEnabled).Append("\n");
            sb.Append("  UsernameClaimName: ").Append(UsernameClaimName).Append("\n");
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
            return this.Equals(input as OauthProfile);
        }

        /// <summary>
        /// Returns true if OauthProfile instances are equal
        /// </summary>
        /// <param name="input">Instance of OauthProfile to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OauthProfile input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AccessLevelGroupsClaimName == input.AccessLevelGroupsClaimName ||
                    (this.AccessLevelGroupsClaimName != null &&
                    this.AccessLevelGroupsClaimName.Equals(input.AccessLevelGroupsClaimName))
                ) && 
                (
                    this.AccessLevelGroupsClaimStringFormat == input.AccessLevelGroupsClaimStringFormat ||
                    (this.AccessLevelGroupsClaimStringFormat != null &&
                    this.AccessLevelGroupsClaimStringFormat.Equals(input.AccessLevelGroupsClaimStringFormat))
                ) && 
                (
                    this.ClientId == input.ClientId ||
                    (this.ClientId != null &&
                    this.ClientId.Equals(input.ClientId))
                ) && 
                (
                    this.ClientRedirectUri == input.ClientRedirectUri ||
                    (this.ClientRedirectUri != null &&
                    this.ClientRedirectUri.Equals(input.ClientRedirectUri))
                ) && 
                (
                    this.ClientRequiredType == input.ClientRequiredType ||
                    (this.ClientRequiredType != null &&
                    this.ClientRequiredType.Equals(input.ClientRequiredType))
                ) && 
                (
                    this.ClientScope == input.ClientScope ||
                    (this.ClientScope != null &&
                    this.ClientScope.Equals(input.ClientScope))
                ) && 
                (
                    this.ClientSecret == input.ClientSecret ||
                    (this.ClientSecret != null &&
                    this.ClientSecret.Equals(input.ClientSecret))
                ) && 
                (
                    this.ClientValidateTypeEnabled == input.ClientValidateTypeEnabled ||
                    (this.ClientValidateTypeEnabled != null &&
                    this.ClientValidateTypeEnabled.Equals(input.ClientValidateTypeEnabled))
                ) && 
                (
                    this.DefaultGlobalAccessLevel == input.DefaultGlobalAccessLevel ||
                    (this.DefaultGlobalAccessLevel != null &&
                    this.DefaultGlobalAccessLevel.Equals(input.DefaultGlobalAccessLevel))
                ) && 
                (
                    this.DefaultMsgVpnAccessLevel == input.DefaultMsgVpnAccessLevel ||
                    (this.DefaultMsgVpnAccessLevel != null &&
                    this.DefaultMsgVpnAccessLevel.Equals(input.DefaultMsgVpnAccessLevel))
                ) && 
                (
                    this.DisplayName == input.DisplayName ||
                    (this.DisplayName != null &&
                    this.DisplayName.Equals(input.DisplayName))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.EndpointAuthorization == input.EndpointAuthorization ||
                    (this.EndpointAuthorization != null &&
                    this.EndpointAuthorization.Equals(input.EndpointAuthorization))
                ) && 
                (
                    this.EndpointDiscovery == input.EndpointDiscovery ||
                    (this.EndpointDiscovery != null &&
                    this.EndpointDiscovery.Equals(input.EndpointDiscovery))
                ) && 
                (
                    this.EndpointDiscoveryRefreshInterval == input.EndpointDiscoveryRefreshInterval ||
                    (this.EndpointDiscoveryRefreshInterval != null &&
                    this.EndpointDiscoveryRefreshInterval.Equals(input.EndpointDiscoveryRefreshInterval))
                ) && 
                (
                    this.EndpointIntrospection == input.EndpointIntrospection ||
                    (this.EndpointIntrospection != null &&
                    this.EndpointIntrospection.Equals(input.EndpointIntrospection))
                ) && 
                (
                    this.EndpointIntrospectionTimeout == input.EndpointIntrospectionTimeout ||
                    (this.EndpointIntrospectionTimeout != null &&
                    this.EndpointIntrospectionTimeout.Equals(input.EndpointIntrospectionTimeout))
                ) && 
                (
                    this.EndpointJwks == input.EndpointJwks ||
                    (this.EndpointJwks != null &&
                    this.EndpointJwks.Equals(input.EndpointJwks))
                ) && 
                (
                    this.EndpointJwksRefreshInterval == input.EndpointJwksRefreshInterval ||
                    (this.EndpointJwksRefreshInterval != null &&
                    this.EndpointJwksRefreshInterval.Equals(input.EndpointJwksRefreshInterval))
                ) && 
                (
                    this.EndpointToken == input.EndpointToken ||
                    (this.EndpointToken != null &&
                    this.EndpointToken.Equals(input.EndpointToken))
                ) && 
                (
                    this.EndpointTokenTimeout == input.EndpointTokenTimeout ||
                    (this.EndpointTokenTimeout != null &&
                    this.EndpointTokenTimeout.Equals(input.EndpointTokenTimeout))
                ) && 
                (
                    this.EndpointUserinfo == input.EndpointUserinfo ||
                    (this.EndpointUserinfo != null &&
                    this.EndpointUserinfo.Equals(input.EndpointUserinfo))
                ) && 
                (
                    this.EndpointUserinfoTimeout == input.EndpointUserinfoTimeout ||
                    (this.EndpointUserinfoTimeout != null &&
                    this.EndpointUserinfoTimeout.Equals(input.EndpointUserinfoTimeout))
                ) && 
                (
                    this.InteractiveEnabled == input.InteractiveEnabled ||
                    (this.InteractiveEnabled != null &&
                    this.InteractiveEnabled.Equals(input.InteractiveEnabled))
                ) && 
                (
                    this.InteractivePromptForExpiredSession == input.InteractivePromptForExpiredSession ||
                    (this.InteractivePromptForExpiredSession != null &&
                    this.InteractivePromptForExpiredSession.Equals(input.InteractivePromptForExpiredSession))
                ) && 
                (
                    this.InteractivePromptForNewSession == input.InteractivePromptForNewSession ||
                    (this.InteractivePromptForNewSession != null &&
                    this.InteractivePromptForNewSession.Equals(input.InteractivePromptForNewSession))
                ) && 
                (
                    this.Issuer == input.Issuer ||
                    (this.Issuer != null &&
                    this.Issuer.Equals(input.Issuer))
                ) && 
                (
                    this.OauthProfileName == input.OauthProfileName ||
                    (this.OauthProfileName != null &&
                    this.OauthProfileName.Equals(input.OauthProfileName))
                ) && 
                (
                    this.OauthRole == input.OauthRole ||
                    (this.OauthRole != null &&
                    this.OauthRole.Equals(input.OauthRole))
                ) && 
                (
                    this.ResourceServerParseAccessTokenEnabled == input.ResourceServerParseAccessTokenEnabled ||
                    (this.ResourceServerParseAccessTokenEnabled != null &&
                    this.ResourceServerParseAccessTokenEnabled.Equals(input.ResourceServerParseAccessTokenEnabled))
                ) && 
                (
                    this.ResourceServerRequiredAudience == input.ResourceServerRequiredAudience ||
                    (this.ResourceServerRequiredAudience != null &&
                    this.ResourceServerRequiredAudience.Equals(input.ResourceServerRequiredAudience))
                ) && 
                (
                    this.ResourceServerRequiredIssuer == input.ResourceServerRequiredIssuer ||
                    (this.ResourceServerRequiredIssuer != null &&
                    this.ResourceServerRequiredIssuer.Equals(input.ResourceServerRequiredIssuer))
                ) && 
                (
                    this.ResourceServerRequiredScope == input.ResourceServerRequiredScope ||
                    (this.ResourceServerRequiredScope != null &&
                    this.ResourceServerRequiredScope.Equals(input.ResourceServerRequiredScope))
                ) && 
                (
                    this.ResourceServerRequiredType == input.ResourceServerRequiredType ||
                    (this.ResourceServerRequiredType != null &&
                    this.ResourceServerRequiredType.Equals(input.ResourceServerRequiredType))
                ) && 
                (
                    this.ResourceServerValidateAudienceEnabled == input.ResourceServerValidateAudienceEnabled ||
                    (this.ResourceServerValidateAudienceEnabled != null &&
                    this.ResourceServerValidateAudienceEnabled.Equals(input.ResourceServerValidateAudienceEnabled))
                ) && 
                (
                    this.ResourceServerValidateIssuerEnabled == input.ResourceServerValidateIssuerEnabled ||
                    (this.ResourceServerValidateIssuerEnabled != null &&
                    this.ResourceServerValidateIssuerEnabled.Equals(input.ResourceServerValidateIssuerEnabled))
                ) && 
                (
                    this.ResourceServerValidateScopeEnabled == input.ResourceServerValidateScopeEnabled ||
                    (this.ResourceServerValidateScopeEnabled != null &&
                    this.ResourceServerValidateScopeEnabled.Equals(input.ResourceServerValidateScopeEnabled))
                ) && 
                (
                    this.ResourceServerValidateTypeEnabled == input.ResourceServerValidateTypeEnabled ||
                    (this.ResourceServerValidateTypeEnabled != null &&
                    this.ResourceServerValidateTypeEnabled.Equals(input.ResourceServerValidateTypeEnabled))
                ) && 
                (
                    this.SempEnabled == input.SempEnabled ||
                    (this.SempEnabled != null &&
                    this.SempEnabled.Equals(input.SempEnabled))
                ) && 
                (
                    this.UsernameClaimName == input.UsernameClaimName ||
                    (this.UsernameClaimName != null &&
                    this.UsernameClaimName.Equals(input.UsernameClaimName))
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
                if (this.AccessLevelGroupsClaimName != null)
                    hashCode = hashCode * 59 + this.AccessLevelGroupsClaimName.GetHashCode();
                if (this.AccessLevelGroupsClaimStringFormat != null)
                    hashCode = hashCode * 59 + this.AccessLevelGroupsClaimStringFormat.GetHashCode();
                if (this.ClientId != null)
                    hashCode = hashCode * 59 + this.ClientId.GetHashCode();
                if (this.ClientRedirectUri != null)
                    hashCode = hashCode * 59 + this.ClientRedirectUri.GetHashCode();
                if (this.ClientRequiredType != null)
                    hashCode = hashCode * 59 + this.ClientRequiredType.GetHashCode();
                if (this.ClientScope != null)
                    hashCode = hashCode * 59 + this.ClientScope.GetHashCode();
                if (this.ClientSecret != null)
                    hashCode = hashCode * 59 + this.ClientSecret.GetHashCode();
                if (this.ClientValidateTypeEnabled != null)
                    hashCode = hashCode * 59 + this.ClientValidateTypeEnabled.GetHashCode();
                if (this.DefaultGlobalAccessLevel != null)
                    hashCode = hashCode * 59 + this.DefaultGlobalAccessLevel.GetHashCode();
                if (this.DefaultMsgVpnAccessLevel != null)
                    hashCode = hashCode * 59 + this.DefaultMsgVpnAccessLevel.GetHashCode();
                if (this.DisplayName != null)
                    hashCode = hashCode * 59 + this.DisplayName.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.EndpointAuthorization != null)
                    hashCode = hashCode * 59 + this.EndpointAuthorization.GetHashCode();
                if (this.EndpointDiscovery != null)
                    hashCode = hashCode * 59 + this.EndpointDiscovery.GetHashCode();
                if (this.EndpointDiscoveryRefreshInterval != null)
                    hashCode = hashCode * 59 + this.EndpointDiscoveryRefreshInterval.GetHashCode();
                if (this.EndpointIntrospection != null)
                    hashCode = hashCode * 59 + this.EndpointIntrospection.GetHashCode();
                if (this.EndpointIntrospectionTimeout != null)
                    hashCode = hashCode * 59 + this.EndpointIntrospectionTimeout.GetHashCode();
                if (this.EndpointJwks != null)
                    hashCode = hashCode * 59 + this.EndpointJwks.GetHashCode();
                if (this.EndpointJwksRefreshInterval != null)
                    hashCode = hashCode * 59 + this.EndpointJwksRefreshInterval.GetHashCode();
                if (this.EndpointToken != null)
                    hashCode = hashCode * 59 + this.EndpointToken.GetHashCode();
                if (this.EndpointTokenTimeout != null)
                    hashCode = hashCode * 59 + this.EndpointTokenTimeout.GetHashCode();
                if (this.EndpointUserinfo != null)
                    hashCode = hashCode * 59 + this.EndpointUserinfo.GetHashCode();
                if (this.EndpointUserinfoTimeout != null)
                    hashCode = hashCode * 59 + this.EndpointUserinfoTimeout.GetHashCode();
                if (this.InteractiveEnabled != null)
                    hashCode = hashCode * 59 + this.InteractiveEnabled.GetHashCode();
                if (this.InteractivePromptForExpiredSession != null)
                    hashCode = hashCode * 59 + this.InteractivePromptForExpiredSession.GetHashCode();
                if (this.InteractivePromptForNewSession != null)
                    hashCode = hashCode * 59 + this.InteractivePromptForNewSession.GetHashCode();
                if (this.Issuer != null)
                    hashCode = hashCode * 59 + this.Issuer.GetHashCode();
                if (this.OauthProfileName != null)
                    hashCode = hashCode * 59 + this.OauthProfileName.GetHashCode();
                if (this.OauthRole != null)
                    hashCode = hashCode * 59 + this.OauthRole.GetHashCode();
                if (this.ResourceServerParseAccessTokenEnabled != null)
                    hashCode = hashCode * 59 + this.ResourceServerParseAccessTokenEnabled.GetHashCode();
                if (this.ResourceServerRequiredAudience != null)
                    hashCode = hashCode * 59 + this.ResourceServerRequiredAudience.GetHashCode();
                if (this.ResourceServerRequiredIssuer != null)
                    hashCode = hashCode * 59 + this.ResourceServerRequiredIssuer.GetHashCode();
                if (this.ResourceServerRequiredScope != null)
                    hashCode = hashCode * 59 + this.ResourceServerRequiredScope.GetHashCode();
                if (this.ResourceServerRequiredType != null)
                    hashCode = hashCode * 59 + this.ResourceServerRequiredType.GetHashCode();
                if (this.ResourceServerValidateAudienceEnabled != null)
                    hashCode = hashCode * 59 + this.ResourceServerValidateAudienceEnabled.GetHashCode();
                if (this.ResourceServerValidateIssuerEnabled != null)
                    hashCode = hashCode * 59 + this.ResourceServerValidateIssuerEnabled.GetHashCode();
                if (this.ResourceServerValidateScopeEnabled != null)
                    hashCode = hashCode * 59 + this.ResourceServerValidateScopeEnabled.GetHashCode();
                if (this.ResourceServerValidateTypeEnabled != null)
                    hashCode = hashCode * 59 + this.ResourceServerValidateTypeEnabled.GetHashCode();
                if (this.SempEnabled != null)
                    hashCode = hashCode * 59 + this.SempEnabled.GetHashCode();
                if (this.UsernameClaimName != null)
                    hashCode = hashCode * 59 + this.UsernameClaimName.GetHashCode();
                return hashCode;
            }
        }
    }
}
