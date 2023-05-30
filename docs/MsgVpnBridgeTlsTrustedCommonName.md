# Semp.V2.CSharp.Model.MsgVpnBridgeTlsTrustedCommonName
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BridgeName** | **string** | The name of the Bridge. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation. | [optional] 
**BridgeVirtualRouter** | **string** | The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt;  Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation. | [optional] 
**TlsTrustedCommonName** | **string** | The expected trusted common name of the remote certificate. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

