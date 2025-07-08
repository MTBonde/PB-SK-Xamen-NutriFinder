# NutriFinder API Documentation

## Overview
NutriFinder is a RESTful API built with ASP.NET Core 9.0 that provides nutritional information for food items. The API serves as a caching layer between clients and external nutrition data sources, with MongoDB as the primary database for caching results.

## API Structure & Architecture

### Core Components
- **Server**: ASP.NET Core 9.0 Web API (`NutriFinder.Server`)
- **Database**: MongoDB integration (`NutriFinder.Database`)
- **Shared Models**: Common DTOs (`Nutrifinder.Shared`)
- **Client Library**: .NET client for consuming the API (`NutriFinderClient`)

### Hosting Configuration
- **Development**: `http://localhost:5062` or `https://localhost:7035`
- **Production**: `http://0.0.0.0:5000` (Docker container)
- **Public Endpoint**: `https://api.mtbonde.dev` (with fallback to localhost)

## API Endpoints

### Main Nutrition Endpoint
**GET** `/api/nutrition?foodItemName={foodItem}`

**Description**: Retrieves nutritional information for a specified food item.

**Parameters**:
- `foodItemName` (query string, required): Name of the food item

**Request Validation**:
- Must not be empty or whitespace
- Cannot contain numbers
- Cannot contain special characters/symbols  
- Must be 1-32 characters long
- Only English letters (a-z, A-Z, å-Å) are accepted
- Uses regex pattern: `^[a-åA-Å]{1,32}$`

**Response Format** (JSON):
```json
{
  "foodItemName": "string",
  "carb": float,
  "fiber": float, 
  "protein": float,
  "fat": float,
  "kcal": float
}
```

**HTTP Status Codes**:
- `200 OK`: Success, nutrition data found
- `400 Bad Request`: Invalid input (fails validation)
- `404 Not Found`: Food item not found in database or external API
- `405 Method Not Allowed`: Wrong HTTP method used
- `503 Service Unavailable`: External API unavailable and no cached data

### Health Check Endpoint
**GET** `/`

**Response**:
```json
{
  "status": "NutriFinder API online",
  "version": "1.0.0"
}
```

## Data Flow & Caching Strategy

1. **Input Validation**: Request validated using `RequestValidator`
2. **Database Lookup**: Check MongoDB cache first
3. **External API Fallback**: If not cached, query external nutrition API
4. **Cache Storage**: Save external API results to MongoDB
5. **Response**: Return nutrition data to client

## Data Sources

### Primary External Data Source
- **DTU Nutrition API**: Uses Excel file (`Frida_5.3_November2024_Dataset.xlsx`)
- **Data Mapping**:
  - "Carbohydrate, available" → Carb
  - "Dietary fibre" → Fiber
  - "Protein" → Protein
  - "Fat" → Fat
  - "Energy (kcal)" → Kcal

### Database
- **MongoDB**: Stores cached nutrition data
- **Connection**: Configurable via `Mongo__ConnectionString` environment variable
- **Default**: `mongodb://localhost:27017`

## Authentication & Authorization
- **Current State**: No authentication implemented
- **Authorization**: Basic ASP.NET Core authorization middleware enabled but not configured
- **Security**: Input validation prevents injection attacks

## Client Integration Examples

### Using the Provided .NET Client
```csharp
var client = new NutritionClient();
var result = await client.FetchWithFallbackBaseUrlsAsync("apple");
```

### Direct HTTP Calls
```bash
# Get nutrition data for an apple
curl "https://api.mtbonde.dev/api/nutrition?foodItemName=apple"

# Health check
curl "https://api.mtbonde.dev/"
```

### JavaScript/Web Integration
```javascript
const response = await fetch('https://api.mtbonde.dev/api/nutrition?foodItemName=apple');
const nutritionData = await response.json();
```

### Python Integration
```python
import requests

response = requests.get('https://api.mtbonde.dev/api/nutrition?foodItemName=apple')
nutrition_data = response.json()
```

## Error Handling
- **Validation Errors**: Return 400 with no body
- **Not Found**: Return 404 when food item doesn't exist
- **Service Errors**: Return 503 when external API is unavailable
- **Logging**: Comprehensive console logging throughout request lifecycle

## Deployment & Infrastructure

### Docker Support
- **Docker Compose**: Includes MongoDB and API server
- **Networks**: Uses `nutrifinder-net` network
- **Environment**: Configurable via environment variables

### Configuration
- **Environment Variables**:
  - `ASPNETCORE_ENVIRONMENT`: Development/Production
  - `Mongo__ConnectionString`: MongoDB connection string

## Use Cases for External Applications

### Fitness & Health Applications
- **Calorie Tracking**: Log food items and automatically get nutritional breakdown
- **Macro Counting**: Track carbohydrates, protein, and fat intake
- **Meal Planning**: Calculate nutritional values for planned meals

### Food & Recipe Applications
- **Recipe Nutrition**: Calculate total nutrition for recipes by querying ingredients
- **Portion Control**: Get nutrition data to help users understand serving sizes
- **Dietary Management**: Help users track specific nutritional goals

### Research & Analytics
- **Nutritional Research**: Access standardized nutrition data for studies
- **Dietary Analysis**: Analyze eating patterns and nutritional intake
- **Health Monitoring**: Track nutritional trends over time

## API Capabilities Summary

### What External Applications Can Do:
1. **Retrieve Nutrition Data**: Get detailed nutritional information for food items
2. **Batch Processing**: Multiple sequential requests for different food items
3. **Caching Benefits**: Faster responses for previously queried items
4. **Error Handling**: Proper HTTP status codes for different scenarios
5. **Health Monitoring**: Check API availability

### Integration Considerations:
- **Rate Limiting**: Not implemented - external applications should implement their own
- **Input Sanitization**: Built-in validation prevents malicious input
- **Response Format**: Consistent JSON structure for all successful responses
- **Fallback Handling**: Built-in fallback mechanisms for high availability

### Limitations:
- **No Authentication**: Open API with no access control
- **Language Support**: Only English food names supported
- **Input Restrictions**: Strict validation limits food name formats
- **No Batch Endpoints**: Single item requests only

## Example Response
```json
{
  "foodItemName": "apple",
  "carb": 13.8,
  "fiber": 2.4,
  "protein": 0.3,
  "fat": 0.2,
  "kcal": 52.0
}
```

## Contact & Support
For questions or issues regarding the API, please refer to the project repository or contact the development team.