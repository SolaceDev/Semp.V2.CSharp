# Semp.V2.CSharp.Model.OauthProfileAccessLevelGroup
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Description** | **string** | A description for the group. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**GlobalAccessLevel** | **string** | The global access level for this group. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to global data. \&quot;read-only\&quot; - User has read-only access to global data. \&quot;read-write\&quot; - User has read-write access to most global data. \&quot;admin\&quot; - User has read-write access to all global data. &lt;/pre&gt;  | [optional] 
**GroupName** | **string** | The name of the group. | [optional] 
**MsgVpnAccessLevel** | **string** | The default message VPN access level for this group. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - User has no access to a Message VPN. \&quot;read-only\&quot; - User has read-only access to a Message VPN. \&quot;read-write\&quot; - User has read-write access to most Message VPN settings. &lt;/pre&gt;  | [optional] 
**OauthProfileName** | **string** | The name of the OAuth profile. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

