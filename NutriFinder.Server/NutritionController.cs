using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriFinder.Database.Interfaces;
using NutriFinder.Server.Helpers;
using NutriFinder.Server.Interfaces;
using Nutrifinder.Shared;

namespace NutriFinder.Server
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionController : ControllerBase
    {
        private INutritionRepository nutritionRepository;
        private INutritionExternalApi _nutritionExternalApi;
        private RequestValidator requestValidator;
        
        public NutritionController(
            INutritionRepository nutritionRepository,
            INutritionExternalApi nutritionExternalApi,
            RequestValidator requestValidator)
        {
            this.nutritionRepository = nutritionRepository;
            this._nutritionExternalApi = nutritionExternalApi;
            this.requestValidator = requestValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string foodItemName)
        {
            Console.WriteLine($"[Controller] Received request for: '{foodItemName}'");

            var validationResult = requestValidator.Validate(foodItemName);
            if (validationResult != "ok")
            {
                Console.WriteLine($"[Controller] Validation failed for: '{foodItemName}' → {validationResult}");
                return BadRequest();
            }

            NutritionDTO dto = null;
            try
            {
                Console.WriteLine($"[Controller] Attempting DB lookup for: '{foodItemName}'");
                dto = await nutritionRepository.GetNutritionDataAsync(foodItemName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Controller] Database error during lookup for '{foodItemName}': {ex.Message}");
            }

            if (dto != null)
            {
                Console.WriteLine($"[Controller] Found in DB: '{foodItemName}'");
                return Ok(dto);
            }

            try
            {
                Console.WriteLine($"[Controller] Not found in DB. Trying external API for: '{foodItemName}'");
                var fetchedDto = await _nutritionExternalApi.FetchNutritionDataAsync(foodItemName);

                if (fetchedDto == null)
                {
                    Console.WriteLine($"[Controller] Not found in external API: '{foodItemName}'");
                    return NotFound();
                }

                try
                {
                    Console.WriteLine($"[Controller] Saving fetched result to DB for: '{foodItemName}'");
                    await nutritionRepository.SaveNutritionDataAsync(fetchedDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Controller] Could not save to DB for '{foodItemName}': {ex.Message}");
                    return Ok(fetchedDto);
                }

                Console.WriteLine($"[Controller] Successfully fetched and saved: '{foodItemName}'");
                return Ok(fetchedDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Controller] External API failed for '{foodItemName}': {e.Message}");
                return StatusCode(503, "Error: External API is not available and no cached data was found.");
            }
        }
    }
}