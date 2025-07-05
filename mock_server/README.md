# Mock Server (JSON-Server)

This directory contains a mock API server powered by [JSON-Server](https://github.com/typicode/json-server) for local development and testing.

## Usage

1. Install dependencies (from the project root):

   ```sh
   npm install
   ```

2. Start the mock server:

   ```sh
   cd mock_server
   npm start
   ```

3. The server will be running at [http://localhost:3000](http://localhost:3000)

- To change the port, use the `--port` flag: `json-server --watch db.json --port 3001`
  - Tip: edit the script in `mock_server/package.json`
- The API endpoints are based on the structure in `db.json` (e.g., `/vehicles`, `/users`).
- You can edit `db.json` to add, remove, or modify mock data.

## Customisation

- To add more resources, simply add new keys to `db.json`.
- For advanced configuration, refer to the [JSON-Server documentation](https://github.com/typicode/json-server#readme).
