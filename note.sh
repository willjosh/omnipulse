# deploying the backend to Azure Container Registry
az login

az acr build --registry omnipulse --image omnipulse-server:latest --file ./server/Dockerfile.server.prod .

# Build locally to see more detailed error messages
docker build -f ./server/Dockerfile.server.prod --platform linux/amd64 -t omnipulse-server .

docker run -d -p 8080:8080 \
  -e "ConnectionStrings__OmnipulseDatabaseConnection=Server=tcp:omnipulsedbserver.database.windows.net,1433;Initial Catalog=Omnipulse;Persist Security Info=False;User ID=CloudSA12de315f;Password=Mamba9909!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  omnipulse-backend

# tag
docker tag omnipulse-server omnipulse.azurecr.io/omnipulse-server:v1

docker push omnipulse.azurecr.io/omnipulse-server:v1

# check if the image is pushed
az acr repository list --name omnipulse --output table

az containerapp show \
  --name omnipulse-backend \
  --resource-group Omnipulse \
  --query "properties.provisioningState" \
  --output tsv

az containerapp create \
  --name omnipulse-backend \
  --resource-group Omnipulse \
  --environment managedEnvironment-Omnipulse-ab52 \
  --image omnipulse.azurecr.io/omnipulse-server:v1 \
  --target-port 8080 \
  --ingress external \
  --registry-server omnipulse.azurecr.io \
  --env-vars "ConnectionStrings__OmnipulseDatabaseConnection=Server=tcp:omnipulsedbserver.database.windows.net,1433;Initial Catalog=Omnipulse;Persist Security Info=False;User ID=CloudSA12de315f;Password=Mamba9909!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --cpu 1.0 \
  --memory 2Gi \
  --min-replicas 1 \
  --max-replicas 3

# Then create with explicit credentials (replace USERNAME and PASSWORD from above)
az containerapp create \
  --name omnipulse-backend \
  --resource-group Omnipulse \
  --environment managedEnvironment-Omnipulse-ab52 \
  --image omnipulse.azurecr.io/omnipulse-server:v1 \
  --target-port 8080 \
  --ingress external \
  --registry-server omnipulse.azurecr.io \
  --registry-username  omnipulse \
  --registry-password  M8vhmKXe9EOU56rG98h9AC6Sh4eIGHZWjpNxKKYCqs+ACRDIJ2Rp  \
  --env-vars "ConnectionStrings__OmnipulseDatabaseConnection=Server=tcp:omnipulsedbserver.database.windows.net,1433;Initial Catalog=Omnipulse;Persist Security Info=False;User ID=CloudSA12de315f;Password=Mamba9909!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --cpu 1.0 \
  --memory 2Gi \
  --min-replicas 1 \
  --max-replicas 3

# CLIENT

docker buildx build -f client/Dockerfile.client.prod \
  --platform linux/amd64 \
  -t omnipulse-client .

# for local testing
docker buildx build -f client/Dockerfile.client.prod \
  -t omnipulse-client .

docker run -d -p 3000:3000 omnipulse-client

# Tag Client
docker tag omnipulse-client omnipulse.azurecr.io/omnipulse-client:v1

# Push Client
docker push omnipulse.azurecr.io/omnipulse-client:v1

az containerapp create \
  --name omnipulse-frontend \
  --resource-group Omnipulse \
  --environment managedEnvironment-Omnipulse-ab52 \
  --image omnipulse.azurecr.io/omnipulse-client:v1 \
  --target-port 3000 \
  --ingress external \
  --registry-server omnipulse.azurecr.io \
  --registry-username  omnipulse \
  --registry-password  M8vhmKXe9EOU56rG98h9AC6Sh4eIGHZWjpNxKKYCqs+ACRDIJ2Rp  \
  --cpu 0.5 \
  --memory 1Gi \
  --min-replicas 1 \
  --max-replicas 2

az containerapp show \
  --name omnipulse-frontend \
  --resource-group Omnipulse \
  --query properties.configuration.ingress.fqdn \
  --output tsv


# UPDATE Backend

# 1. Build with same tag
docker build -f server/Dockerfile.server.prod --platform linux/amd64 -t omnipulse.azurecr.io/omnipulse-server:v1 .

# 2. Push (overwrites previous v1)
docker push omnipulse.azurecr.io/omnipulse-server:v1

# 3. update the container app
az containerapp update \
  --name omnipulse-backend \
  --resource-group Omnipulse \
  --image omnipulse.azurecr.io/omnipulse-server:v1


# Update Frontend

docker build -f client/Dockerfile.client.prod --platform linux/amd64 -t omnipulse.azurecr.io/omnipulse-client:v1 .

docker push omnipulse.azurecr.io/omnipulse-client:v1

az containerapp update \
  --name omnipulse-frontend \
  --resource-group Omnipulse \
  --image omnipulse.azurecr.io/omnipulse-client:v1