# Semp.V2.CSharp.Model.OauthProfile
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccessLevelGroupsClaimName** | **string** | The name of the groups claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;groups\&quot;&#x60;. | [optional] 
**AccessLevelGroupsClaimStringFormat** | **string** | The format of the access level groups claim value when it is a string. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;single\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;single\&quot; - When the claim is a string, it is interpreted as a single group. \&quot;space-delimited\&quot; - When the claim is a string, it is interpreted as a space-delimited list of groups, similar to the \&quot;scope\&quot; claim. &lt;/pre&gt;  Available since 2.32. | [optional] 
**ClientId** | **string** | The OAuth client id. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ClientRedirectUri** | **string** | The OAuth redirect URI. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ClientRequiredType** | **string** | The required value for the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;JWT\&quot;&#x60;. | [optional] 
**ClientScope** | **string** | The OAuth scope. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;openid email\&quot;&#x60;. | [optional] 
**ClientSecret** | **string** | The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ClientValidateTypeEnabled** | **bool?** | Enable or disable verification of the TYP field in the ID token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**DefaultGlobalAccessLevel** | **string** | The default global access level for this OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt;  | [optional] 
**DefaultMsgVpnAccessLevel** | **string** | The default message VPN access level for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt;  | [optional] 
**DisplayName** | **string** | The user friendly name for the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**Enabled** | **bool?** | Enable or disable the OAuth profile. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**EndpointAuthorization** | **string** | The OAuth authorization endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointDiscovery** | **string** | The OpenID Connect discovery endpoint or OAuth Authorization Server Metadata endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointDiscoveryRefreshInterval** | **int?** | The number of seconds between discovery endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;. | [optional] 
**EndpointIntrospection** | **string** | The OAuth introspection endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointIntrospectionTimeout** | **int?** | The maximum time in seconds a token introspection request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;. | [optional] 
**EndpointJwks** | **string** | The OAuth JWKS endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointJwksRefreshInterval** | **int?** | The number of seconds between JWKS endpoint requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;. | [optional] 
**EndpointToken** | **string** | The OAuth token endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointTokenTimeout** | **int?** | The maximum time in seconds a token request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;. | [optional] 
**EndpointUserinfo** | **string** | The OpenID Connect Userinfo endpoint. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**EndpointUserinfoTimeout** | **int?** | The maximum time in seconds a userinfo request is allowed to take. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;. | [optional] 
**InteractiveEnabled** | **bool?** | Enable or disable interactive logins via this OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**InteractivePromptForExpiredSession** | **string** | The value of the prompt parameter provided to the OAuth authorization server for login requests where the session has expired. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**InteractivePromptForNewSession** | **string** | The value of the prompt parameter provided to the OAuth authorization server for login requests where the session is new or the user has explicitly logged out. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;select_account\&quot;&#x60;. | [optional] 
**Issuer** | **string** | The Issuer Identifier for the OAuth provider. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**OauthProfileName** | **string** | The name of the OAuth profile. | [optional] 
**OauthRole** | **string** | The OAuth role of the broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;client\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;client\&quot; - The broker is in the OAuth client role. \&quot;resource-server\&quot; - The broker is in the OAuth resource server role. &lt;/pre&gt;  | [optional] 
**ResourceServerParseAccessTokenEnabled** | **bool?** | Enable or disable parsing of the access token as a JWT. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**ResourceServerRequiredAudience** | **string** | The required audience value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ResourceServerRequiredIssuer** | **string** | The required issuer value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ResourceServerRequiredScope** | **string** | A space-separated list of scopes that must be present in the scope claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**ResourceServerRequiredType** | **string** | The required TYP value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;at+jwt\&quot;&#x60;. | [optional] 
**ResourceServerValidateAudienceEnabled** | **bool?** | Enable or disable verification of the audience claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**ResourceServerValidateIssuerEnabled** | **bool?** | Enable or disable verification of the issuer claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**ResourceServerValidateScopeEnabled** | **bool?** | Enable or disable verification of the scope claim in the access token or introspection response. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**ResourceServerValidateTypeEnabled** | **bool?** | Enable or disable verification of the TYP field in the access token header. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**SempEnabled** | **bool?** | Enable or disable authentication of SEMP requests with OAuth tokens. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. | [optional] 
**UsernameClaimName** | **string** | The name of the username claim. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;sub\&quot;&#x60;. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
