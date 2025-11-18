### User Roles

| Role | Description | Typical Permissions |
|---------|--------------|----------------------|
| `Admin` | Full system control. | Manage users, products, orders, and application settings. |
| `Publisher` | Can create, update, and publish products. | Manage products and orders related to the products. |
| `Customer` | Regular user who can browse and purchase. | View products, place orders, and manage their own profile and orders. |

* **Publisher :** Can manage all products in the platform. Usually not ideal but for this simple site it's ok.
* **Customer :** Not really needed until a store functionality is needed in the future or never. 
 
### ENV

* `ConnectionStrings__DefaultConnection` Default Postgres connection string. In the form: `Host=localhost;Port=5432;Database=ConfeccionesAlbaDb;Username=devuser;Password=devpass`.

* `InitialAdmin__Name` Initial admin name
* `InitialAdmin__Email` Initial admin email
* `InitialAdmin__Password` initial admin password

* `JwtSettings__SecretKey` Default Jwt Secret (Setup ASAP)
* `JwtSettings__ExpirationInMinutes` Expiration time in minutes of the Jwt token
* `JwtSettings__Issuer` Issuer
* `JwtSettings__Audience` Audience

* `AllowedOrigins__0` CORS Allowed Origin

* `Cloudflare__R2__AccountId` Account ID
* `Cloudflare__R2__AccessKey` Access Key
* `Cloudflare__R2__SecretKey` Secret Key
* `Cloudflare__R2__BucketName` Bucket Name
* `Cloudflare__R2__EndpointUrl` S3 API
* `Cloudflare__R2__PublicUrl` Public Url

![Alt](https://repobeats.axiom.co/api/embed/7d418bd249040596bcd0fbf34261db3ed922faa0.svg "Repobeats analytics image")
