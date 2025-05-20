using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NutriFinder.Server.Helpers;
using NutriFinder.Server.Interfaces;

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
        public async Task<IActionResult> Get([FromQuery]string foodItemName)
        {
            // EO; input validation
            var validationResult = requestValidator.Validate(foodItemName);
            if (validationResult != "ok")
            {
                return BadRequest();
            }
            
            // lookup in Database
            var dto = await nutritionRepository.GetNutritionDataAsync(foodItemName);
            if (dto != null)
                return Ok(dto);

            // try lookup with external api
            try
            {
                //Attempt fetch of data from external
                var fetchedDto = await _nutritionExternalApi.FetchNutritionDataAsync(foodItemName);
                if (fetchedDto == null)
                {
                    return NotFound();
                }
                
                // save to local database and return result
                await nutritionRepository.SaveNutritionDataAsync(fetchedDto);
                return Ok(fetchedDto);
            }
            // catch exception if not available and return 503
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(503, "Error: External API is not available and no cached data was found.");
            }
        }
    }
}