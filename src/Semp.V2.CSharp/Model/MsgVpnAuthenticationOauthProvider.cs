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
    /// MsgVpnAuthenticationOauthProvider
    /// </summary>
    [DataContract]
        public partial class MsgVpnAuthenticationOauthProvider :  IEquatable<MsgVpnAuthenticationOauthProvider>
    {
        /// <summary>
        /// The audience claim source, indicating where to search for the audience value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The audience claim source, indicating where to search for the audience value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AudienceClaimSourceEnum
        {
            /// <summary>
            /// Enum AccessToken for value: access-token
            /// </summary>
            [EnumMember(Value = "access-token")]
            AccessToken = 1,
            /// <summary>
            /// Enum IdToken for value: id-token
            /// </summary>
            [EnumMember(Value = "id-token")]
            IdToken = 2,
            /// <summary>
            /// Enum Introspection for value: introspection
            /// </summary>
            [EnumMember(Value = "introspection")]
            Introspection = 3        }
        /// <summary>
        /// The audience claim source, indicating where to search for the audience value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The audience claim source, indicating where to search for the audience value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="audienceClaimSource", EmitDefaultValue=false)]
        public AudienceClaimSourceEnum? AudienceClaimSource { get; set; }
        /// <summary>
        /// The authorization group claim source, indicating where to search for the authorization group name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The authorization group claim source, indicating where to search for the authorization group name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthorizationGroupClaimSourceEnum
        {
            /// <summary>
            /// Enum AccessToken for value: access-token
            /// </summary>
            [EnumMember(Value = "access-token")]
            AccessToken = 1,
            /// <summary>
            /// Enum IdToken for value: id-token
            /// </summary>
            [EnumMember(Value = "id-token")]
            IdToken = 2,
            /// <summary>
            /// Enum Introspection for value: introspection
            /// </summary>
            [EnumMember(Value = "introspection")]
            Introspection = 3        }
        /// <summary>
        /// The authorization group claim source, indicating where to search for the authorization group name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The authorization group claim source, indicating where to search for the authorization group name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="authorizationGroupClaimSource", EmitDefaultValue=false)]
        public AuthorizationGroupClaimSourceEnum? AuthorizationGroupClaimSource { get; set; }
        /// <summary>
        /// The username claim source, indicating where to search for the username value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The username claim source, indicating where to search for the username value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum UsernameClaimSourceEnum
        {
            /// <summary>
            /// Enum AccessToken for value: access-token
            /// </summary>
            [EnumMember(Value = "access-token")]
            AccessToken = 1,
            /// <summary>
            /// Enum IdToken for value: id-token
            /// </summary>
            [EnumMember(Value = "id-token")]
            IdToken = 2,
            /// <summary>
            /// Enum Introspection for value: introspection
            /// </summary>
            [EnumMember(Value = "introspection")]
            Introspection = 3        }
        /// <summary>
        /// The username claim source, indicating where to search for the username value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The username claim source, indicating where to search for the username value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="usernameClaimSource", EmitDefaultValue=false)]
        public UsernameClaimSourceEnum? UsernameClaimSource { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnAuthenticationOauthProvider" /> class.
        /// </summary>
        /// <param name="audienceClaimName">The audience claim name, indicating which part of the object to use for determining the audience. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;aud\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="audienceClaimSource">The audience claim source, indicating where to search for the audience value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="audienceClaimValue">The required audience value for a token to be considered valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="audienceValidationEnabled">Enable or disable audience validation. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="authorizationGroupClaimName">The authorization group claim name, indicating which part of the object to use for determining the authorization group. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;scope\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="authorizationGroupClaimSource">The authorization group claim source, indicating where to search for the authorization group name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="authorizationGroupEnabled">Enable or disable OAuth based authorization. When enabled, the configured authorization type for OAuth clients is overridden. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="disconnectOnTokenExpirationEnabled">Enable or disable the disconnection of clients when their tokens expire. Changing this value does not affect existing clients, only new client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="enabled">Enable or disable OAuth Provider client authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="jwksRefreshInterval">The number of seconds between forced JWKS public key refreshing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;86400&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="jwksUri">The URI where the OAuth provider publishes its JWKS public keys. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="msgVpnName">The name of the Message VPN. Deprecated since 2.25. Replaced by authenticationOauthProfiles..</param>
        /// <param name="oauthProviderName">The name of the OAuth Provider. Deprecated since 2.25. Replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIgnoreTimeLimitsEnabled">Enable or disable whether to ignore time limits and accept tokens that are not yet valid or are no longer valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIntrospectionParameterName">The parameter name used to identify the token during access token introspection. A standards compliant OAuth introspection server expects \&quot;token\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;token\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIntrospectionPassword">The password to use when logging into the token introspection URI. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIntrospectionTimeout">The maximum time in seconds a token introspection is allowed to take. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIntrospectionUri">The token introspection URI of the OAuth authentication server. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="tokenIntrospectionUsername">The username to use when logging into the token introspection URI. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="usernameClaimName">The username claim name, indicating which part of the object to use for determining the username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="usernameClaimSource">The username claim source, indicating where to search for the username value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;id-token\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;access-token\&quot; - The OAuth v2 access_token. \&quot;id-token\&quot; - The OpenID Connect id_token. \&quot;introspection\&quot; - The result of introspecting the OAuth v2 access_token. &lt;/pre&gt;  Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        /// <param name="usernameValidateEnabled">Enable or disable whether the API provided username will be validated against the username calculated from the token(s); the connection attempt is rejected if they differ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles..</param>
        public MsgVpnAuthenticationOauthProvider(string audienceClaimName = default(string), AudienceClaimSourceEnum? audienceClaimSource = default(AudienceClaimSourceEnum?), string audienceClaimValue = default(string), bool? audienceValidationEnabled = default(bool?), string authorizationGroupClaimName = default(string), AuthorizationGroupClaimSourceEnum? authorizationGroupClaimSource = default(AuthorizationGroupClaimSourceEnum?), bool? authorizationGroupEnabled = default(bool?), bool? disconnectOnTokenExpirationEnabled = default(bool?), bool? enabled = default(bool?), int? jwksRefreshInterval = default(int?), string jwksUri = default(string), string msgVpnName = default(string), string oauthProviderName = default(string), bool? tokenIgnoreTimeLimitsEnabled = default(bool?), string tokenIntrospectionParameterName = default(string), string tokenIntrospectionPassword = default(string), int? tokenIntrospectionTimeout = default(int?), string tokenIntrospectionUri = default(string), string tokenIntrospectionUsername = default(string), string usernameClaimName = default(string), UsernameClaimSourceEnum? usernameClaimSource = default(UsernameClaimSourceEnum?), bool? usernameValidateEnabled = default(bool?))
        {
            this.AudienceClaimName = audienceClaimName;
            this.AudienceClaimSource = audienceClaimSource;
            this.AudienceClaimValue = audienceClaimValue;
            this.AudienceValidationEnabled = audienceValidationEnabled;
            this.AuthorizationGroupClaimName = authorizationGroupClaimName;
            this.AuthorizationGroupClaimSource = authorizationGroupClaimSource;
            this.AuthorizationGroupEnabled = authorizationGroupEnabled;
            this.DisconnectOnTokenExpirationEnabled = disconnectOnTokenExpirationEnabled;
            this.Enabled = enabled;
            this.JwksRefreshInterval = jwksRefreshInterval;
            this.JwksUri = jwksUri;
            this.MsgVpnName = msgVpnName;
            this.OauthProviderName = oauthProviderName;
            this.TokenIgnoreTimeLimitsEnabled = tokenIgnoreTimeLimitsEnabled;
            this.TokenIntrospectionParameterName = tokenIntrospectionParameterName;
            this.TokenIntrospectionPassword = tokenIntrospectionPassword;
            this.TokenIntrospectionTimeout = tokenIntrospectionTimeout;
            this.TokenIntrospectionUri = tokenIntrospectionUri;
            this.TokenIntrospectionUsername = tokenIntrospectionUsername;
            this.UsernameClaimName = usernameClaimName;
            this.UsernameClaimSource = usernameClaimSource;
            this.UsernameValidateEnabled = usernameValidateEnabled;
        }
        
        /// <summary>
        /// The audience claim name, indicating which part of the object to use for determining the audience. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;aud\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The audience claim name, indicating which part of the object to use for determining the audience. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;aud\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="audienceClaimName", EmitDefaultValue=false)]
        public string AudienceClaimName { get; set; }


        /// <summary>
        /// The required audience value for a token to be considered valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The required audience value for a token to be considered valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="audienceClaimValue", EmitDefaultValue=false)]
        public string AudienceClaimValue { get; set; }

        /// <summary>
        /// Enable or disable audience validation. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable audience validation. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="audienceValidationEnabled", EmitDefaultValue=false)]
        public bool? AudienceValidationEnabled { get; set; }

        /// <summary>
        /// The authorization group claim name, indicating which part of the object to use for determining the authorization group. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;scope\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The authorization group claim name, indicating which part of the object to use for determining the authorization group. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;scope\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="authorizationGroupClaimName", EmitDefaultValue=false)]
        public string AuthorizationGroupClaimName { get; set; }


        /// <summary>
        /// Enable or disable OAuth based authorization. When enabled, the configured authorization type for OAuth clients is overridden. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable OAuth based authorization. When enabled, the configured authorization type for OAuth clients is overridden. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="authorizationGroupEnabled", EmitDefaultValue=false)]
        public bool? AuthorizationGroupEnabled { get; set; }

        /// <summary>
        /// Enable or disable the disconnection of clients when their tokens expire. Changing this value does not affect existing clients, only new client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable the disconnection of clients when their tokens expire. Changing this value does not affect existing clients, only new client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="disconnectOnTokenExpirationEnabled", EmitDefaultValue=false)]
        public bool? DisconnectOnTokenExpirationEnabled { get; set; }

        /// <summary>
        /// Enable or disable OAuth Provider client authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable OAuth Provider client authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// The number of seconds between forced JWKS public key refreshing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;86400&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The number of seconds between forced JWKS public key refreshing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;86400&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="jwksRefreshInterval", EmitDefaultValue=false)]
        public int? JwksRefreshInterval { get; set; }

        /// <summary>
        /// The URI where the OAuth provider publishes its JWKS public keys. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The URI where the OAuth provider publishes its JWKS public keys. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="jwksUri", EmitDefaultValue=false)]
        public string JwksUri { get; set; }

        /// <summary>
        /// The name of the Message VPN. Deprecated since 2.25. Replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The name of the Message VPN. Deprecated since 2.25. Replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The name of the OAuth Provider. Deprecated since 2.25. Replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The name of the OAuth Provider. Deprecated since 2.25. Replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="oauthProviderName", EmitDefaultValue=false)]
        public string OauthProviderName { get; set; }

        /// <summary>
        /// Enable or disable whether to ignore time limits and accept tokens that are not yet valid or are no longer valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable whether to ignore time limits and accept tokens that are not yet valid or are no longer valid. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIgnoreTimeLimitsEnabled", EmitDefaultValue=false)]
        public bool? TokenIgnoreTimeLimitsEnabled { get; set; }

        /// <summary>
        /// The parameter name used to identify the token during access token introspection. A standards compliant OAuth introspection server expects \&quot;token\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;token\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The parameter name used to identify the token during access token introspection. A standards compliant OAuth introspection server expects \&quot;token\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;token\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIntrospectionParameterName", EmitDefaultValue=false)]
        public string TokenIntrospectionParameterName { get; set; }

        /// <summary>
        /// The password to use when logging into the token introspection URI. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The password to use when logging into the token introspection URI. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIntrospectionPassword", EmitDefaultValue=false)]
        public string TokenIntrospectionPassword { get; set; }

        /// <summary>
        /// The maximum time in seconds a token introspection is allowed to take. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The maximum time in seconds a token introspection is allowed to take. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIntrospectionTimeout", EmitDefaultValue=false)]
        public int? TokenIntrospectionTimeout { get; set; }

        /// <summary>
        /// The token introspection URI of the OAuth authentication server. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The token introspection URI of the OAuth authentication server. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIntrospectionUri", EmitDefaultValue=false)]
        public string TokenIntrospectionUri { get; set; }

        /// <summary>
        /// The username to use when logging into the token introspection URI. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The username to use when logging into the token introspection URI. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="tokenIntrospectionUsername", EmitDefaultValue=false)]
        public string TokenIntrospectionUsername { get; set; }

        /// <summary>
        /// The username claim name, indicating which part of the object to use for determining the username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>The username claim name, indicating which part of the object to use for determining the username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="usernameClaimName", EmitDefaultValue=false)]
        public string UsernameClaimName { get; set; }


        /// <summary>
        /// Enable or disable whether the API provided username will be validated against the username calculated from the token(s); the connection attempt is rejected if they differ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.
        /// </summary>
        /// <value>Enable or disable whether the API provided username will be validated against the username calculated from the token(s); the connection attempt is rejected if they differ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.25. authenticationOauthProviders replaced by authenticationOauthProfiles.</value>
        [DataMember(Name="usernameValidateEnabled", EmitDefaultValue=false)]
        public bool? UsernameValidateEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnAuthenticationOauthProvider {\n");
            sb.Append("  AudienceClaimName: ").Append(AudienceClaimName).Append("\n");
            sb.Append("  AudienceClaimSource: ").Append(AudienceClaimSource).Append("\n");
            sb.Append("  AudienceClaimValue: ").Append(AudienceClaimValue).Append("\n");
            sb.Append("  AudienceValidationEnabled: ").Append(AudienceValidationEnabled).Append("\n");
            sb.Append("  AuthorizationGroupClaimName: ").Append(AuthorizationGroupClaimName).Append("\n");
            sb.Append("  AuthorizationGroupClaimSource: ").Append(AuthorizationGroupClaimSource).Append("\n");
            sb.Append("  AuthorizationGroupEnabled: ").Append(AuthorizationGroupEnabled).Append("\n");
            sb.Append("  DisconnectOnTokenExpirationEnabled: ").Append(DisconnectOnTokenExpirationEnabled).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  JwksRefreshInterval: ").Append(JwksRefreshInterval).Append("\n");
            sb.Append("  JwksUri: ").Append(JwksUri).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  OauthProviderName: ").Append(OauthProviderName).Append("\n");
            sb.Append("  TokenIgnoreTimeLimitsEnabled: ").Append(TokenIgnoreTimeLimitsEnabled).Append("\n");
            sb.Append("  TokenIntrospectionParameterName: ").Append(TokenIntrospectionParameterName).Append("\n");
            sb.Append("  TokenIntrospectionPassword: ").Append(TokenIntrospectionPassword).Append("\n");
            sb.Append("  TokenIntrospectionTimeout: ").Append(TokenIntrospectionTimeout).Append("\n");
            sb.Append("  TokenIntrospectionUri: ").Append(TokenIntrospectionUri).Append("\n");
            sb.Append("  TokenIntrospectionUsername: ").Append(TokenIntrospectionUsername).Append("\n");
            sb.Append("  UsernameClaimName: ").Append(UsernameClaimName).Append("\n");
            sb.Append("  UsernameClaimSource: ").Append(UsernameClaimSource).Append("\n");
            sb.Append("  UsernameValidateEnabled: ").Append(UsernameValidateEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnAuthenticationOauthProvider);
        }

        /// <summary>
        /// Returns true if MsgVpnAuthenticationOauthProvider instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnAuthenticationOauthProvider to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnAuthenticationOauthProvider input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AudienceClaimName == input.AudienceClaimName ||
                    (this.AudienceClaimName != null &&
                    this.AudienceClaimName.Equals(input.AudienceClaimName))
                ) && 
                (
                    this.AudienceClaimSource == input.AudienceClaimSource ||
                    (this.AudienceClaimSource != null &&
                    this.AudienceClaimSource.Equals(input.AudienceClaimSource))
                ) && 
                (
                    this.AudienceClaimValue == input.AudienceClaimValue ||
                    (this.AudienceClaimValue != null &&
                    this.AudienceClaimValue.Equals(input.AudienceClaimValue))
                ) && 
                (
                    this.AudienceValidationEnabled == input.AudienceValidationEnabled ||
                    (this.AudienceValidationEnabled != null &&
                    this.AudienceValidationEnabled.Equals(input.AudienceValidationEnabled))
                ) && 
                (
                    this.AuthorizationGroupClaimName == input.AuthorizationGroupClaimName ||
                    (this.AuthorizationGroupClaimName != null &&
                    this.AuthorizationGroupClaimName.Equals(input.AuthorizationGroupClaimName))
                ) && 
                (
                    this.AuthorizationGroupClaimSource == input.AuthorizationGroupClaimSource ||
                    (this.AuthorizationGroupClaimSource != null &&
                    this.AuthorizationGroupClaimSource.Equals(input.AuthorizationGroupClaimSource))
                ) && 
                (
                    this.AuthorizationGroupEnabled == input.AuthorizationGroupEnabled ||
                    (this.AuthorizationGroupEnabled != null &&
                    this.AuthorizationGroupEnabled.Equals(input.AuthorizationGroupEnabled))
                ) && 
                (
                    this.DisconnectOnTokenExpirationEnabled == input.DisconnectOnTokenExpirationEnabled ||
                    (this.DisconnectOnTokenExpirationEnabled != null &&
                    this.DisconnectOnTokenExpirationEnabled.Equals(input.DisconnectOnTokenExpirationEnabled))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.JwksRefreshInterval == input.JwksRefreshInterval ||
                    (this.JwksRefreshInterval != null &&
                    this.JwksRefreshInterval.Equals(input.JwksRefreshInterval))
                ) && 
                (
                    this.JwksUri == input.JwksUri ||
                    (this.JwksUri != null &&
                    this.JwksUri.Equals(input.JwksUri))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.OauthProviderName == input.OauthProviderName ||
                    (this.OauthProviderName != null &&
                    this.OauthProviderName.Equals(input.OauthProviderName))
                ) && 
                (
                    this.TokenIgnoreTimeLimitsEnabled == input.TokenIgnoreTimeLimitsEnabled ||
                    (this.TokenIgnoreTimeLimitsEnabled != null &&
                    this.TokenIgnoreTimeLimitsEnabled.Equals(input.TokenIgnoreTimeLimitsEnabled))
                ) && 
                (
                    this.TokenIntrospectionParameterName == input.TokenIntrospectionParameterName ||
                    (this.TokenIntrospectionParameterName != null &&
                    this.TokenIntrospectionParameterName.Equals(input.TokenIntrospectionParameterName))
                ) && 
                (
                    this.TokenIntrospectionPassword == input.TokenIntrospectionPassword ||
                    (this.TokenIntrospectionPassword != null &&
                    this.TokenIntrospectionPassword.Equals(input.TokenIntrospectionPassword))
                ) && 
                (
                    this.TokenIntrospectionTimeout == input.TokenIntrospectionTimeout ||
                    (this.TokenIntrospectionTimeout != null &&
                    this.TokenIntrospectionTimeout.Equals(input.TokenIntrospectionTimeout))
                ) && 
                (
                    this.TokenIntrospectionUri == input.TokenIntrospectionUri ||
                    (this.TokenIntrospectionUri != null &&
                    this.TokenIntrospectionUri.Equals(input.TokenIntrospectionUri))
                ) && 
                (
                    this.TokenIntrospectionUsername == input.TokenIntrospectionUsername ||
                    (this.TokenIntrospectionUsername != null &&
                    this.TokenIntrospectionUsername.Equals(input.TokenIntrospectionUsername))
                ) && 
                (
                    this.UsernameClaimName == input.UsernameClaimName ||
                    (this.UsernameClaimName != null &&
                    this.UsernameClaimName.Equals(input.UsernameClaimName))
                ) && 
                (
                    this.UsernameClaimSource == input.UsernameClaimSource ||
                    (this.UsernameClaimSource != null &&
                    this.UsernameClaimSource.Equals(input.UsernameClaimSource))
                ) && 
                (
                    this.UsernameValidateEnabled == input.UsernameValidateEnabled ||
                    (this.UsernameValidateEnabled != null &&
                    this.UsernameValidateEnabled.Equals(input.UsernameValidateEnabled))
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
                if (this.AudienceClaimName != null)
                    hashCode = hashCode * 59 + this.AudienceClaimName.GetHashCode();
                if (this.AudienceClaimSource != null)
                    hashCode = hashCode * 59 + this.AudienceClaimSource.GetHashCode();
                if (this.AudienceClaimValue != null)
                    hashCode = hashCode * 59 + this.AudienceClaimValue.GetHashCode();
                if (this.AudienceValidationEnabled != null)
                    hashCode = hashCode * 59 + this.AudienceValidationEnabled.GetHashCode();
                if (this.AuthorizationGroupClaimName != null)
                    hashCode = hashCode * 59 + this.AuthorizationGroupClaimName.GetHashCode();
                if (this.AuthorizationGroupClaimSource != null)
                    hashCode = hashCode * 59 + this.AuthorizationGroupClaimSource.GetHashCode();
                if (this.AuthorizationGroupEnabled != null)
                    hashCode = hashCode * 59 + this.AuthorizationGroupEnabled.GetHashCode();
                if (this.DisconnectOnTokenExpirationEnabled != null)
                    hashCode = hashCode * 59 + this.DisconnectOnTokenExpirationEnabled.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.JwksRefreshInterval != null)
                    hashCode = hashCode * 59 + this.JwksRefreshInterval.GetHashCode();
                if (this.JwksUri != null)
                    hashCode = hashCode * 59 + this.JwksUri.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.OauthProviderName != null)
                    hashCode = hashCode * 59 + this.OauthProviderName.GetHashCode();
                if (this.TokenIgnoreTimeLimitsEnabled != null)
                    hashCode = hashCode * 59 + this.TokenIgnoreTimeLimitsEnabled.GetHashCode();
                if (this.TokenIntrospectionParameterName != null)
                    hashCode = hashCode * 59 + this.TokenIntrospectionParameterName.GetHashCode();
                if (this.TokenIntrospectionPassword != null)
                    hashCode = hashCode * 59 + this.TokenIntrospectionPassword.GetHashCode();
                if (this.TokenIntrospectionTimeout != null)
                    hashCode = hashCode * 59 + this.TokenIntrospectionTimeout.GetHashCode();
                if (this.TokenIntrospectionUri != null)
                    hashCode = hashCode * 59 + this.TokenIntrospectionUri.GetHashCode();
                if (this.TokenIntrospectionUsername != null)
                    hashCode = hashCode * 59 + this.TokenIntrospectionUsername.GetHashCode();
                if (this.UsernameClaimName != null)
                    hashCode = hashCode * 59 + this.UsernameClaimName.GetHashCode();
                if (this.UsernameClaimSource != null)
                    hashCode = hashCode * 59 + this.UsernameClaimSource.GetHashCode();
                if (this.UsernameValidateEnabled != null)
                    hashCode = hashCode * 59 + this.UsernameValidateEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
