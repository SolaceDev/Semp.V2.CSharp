# Semp.V2.CSharp.Model.MsgVpnBridgeRemoteSubscription
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BridgeName** | **string** | The name of the Bridge. | [optional] 
**BridgeVirtualRouter** | **string** | The virtual router of the Bridge. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The Bridge is used for the primary virtual router. \&quot;backup\&quot; - The Bridge is used for the backup virtual router. \&quot;auto\&quot; - The Bridge is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt;  | [optional] 
**DeliverAlwaysEnabled** | **bool?** | Enable or disable deliver-always for the Bridge remote subscription topic instead of a deliver-to-one remote priority. A given topic for the Bridge may be deliver-to-one or deliver-always but not both. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**RemoteSubscriptionTopic** | **string** | The topic of the Bridge remote subscription. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

