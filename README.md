# net-mvc-fw48-oidc-azure
.Net MVC Framework 4.8 application for testing OIDC with Azure.


Steps for using/testing:

1. Make sure you have a free Windows Live or Azure-account.
 Login to https://portal.azure.com/

2. Navigate to "Microsoft Entra ID" and click "App registrations".

3. Start registration of a new app, i chose the option "Accounts in any organizational directory (Any Microsoft Entra ID tenant - Multitenant)"
   Set the value for "Redirect URIs" to "https://localhost:44339/Home/About"
   Tick the checkbox "ID tokens (used for implicit and hybrid flows)".

4. Navigate to "Certificates & secrets" and create a new "client secret".
   Set a description that you remember, this value should be set in "web.config" for the solution (the key-name in web.config is "ClientSecret".

5. Find the values for "Tenant" and "ClientId" and enter these values into "web.config".

6. Run the solution and try the "login-link", followed by the link to show "Claims"
