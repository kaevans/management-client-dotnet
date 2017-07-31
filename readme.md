# Management Client Sample

This sample will enable retry logic using the Polly framework to retry after a 429 error containing a Retry-After header.

This is based on the GitHub thread for Polly (https://github.com/App-vNext/Polly/issues/247).

## Configuring
To configure the application, open the App.config file and edit the following values:

* subscriptionId - The Azure subscription ID (az account list)
* location - The Azure location (az account list-locations)
* ida:clientId - The Azure AD registered application Application ID (az ad app list --query "[?displayName=='ManagementClient'].appId" )
* ida:clientSecret - The secret key created when the application was registered
* ida:tenant - The Azure tenant ID ( az account list --query "[?name=='Kirk Evans Azure'].tenantId" )

## Testing

To test, install Telerik Fiddler.  Once installed, go to the Rules menu and choose Customize Rules. 
This will override the actual response and force a 429 error.  

```
    static function OnBeforeResponse(oSession: Session) {
        if (m_Hide304s && oSession.responseCode == 304) {
            oSession["ui-hide"] = "true";
        }
		
			
		if ((oSession.hostname == "management.azure.com") 
		&& (oSession.PathAndQuery.Contains("providers/Microsoft.Network"))) {
			oSession.responseCode = 429;  
			oSession.utilSetResponseBody("{'Error':{'Details':[{'Code':'RetryableErrorDueToTooManyCalls','Message':'Subscription 678fbfe2-79a7-4689-839d-0a187e5b6b1a was used to perform too many calls within last 5 minutes. The number of calls exceeds Microsoft.Network throttling limit. The call can be retried in 39 seconds.','Target':null}],'InnerError':null,'Code':'RetryableError','Message':'A retryable error occurred.','Target':null}}");
			oSession.oResponse["Cache-Control"] = "no-cache";
			oSession.oResponse["Pragma"] = "no-cache";
			oSession.oResponse["Content-Type"]="application/json; charset=utf-8";
			oSession.oResponse["Expires"] = "-1";
			oSession.oResponse["Retry-After"] = "39";
			oSession.oResponse["Server"] = "Microsoft-HTTPAPI/2.0";
			oSession.oResponse["x-ms-ratelimit-remaining-subscription-writes"] = "5";								
	    }	
    }
```
