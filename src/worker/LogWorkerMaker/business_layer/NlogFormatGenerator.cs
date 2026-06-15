using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LogWorkerMaker.business_layer
{
    public class NlogFormatGenerator
    {

        private readonly ILogger<NlogFormatGenerator> _logger;

        public NlogFormatGenerator(ILogger<NlogFormatGenerator> logger)
        {
            _logger = logger;
        }

        public void log_simulator()
        {

            _logger.LogInformation("Start simulation");

            try
            {
                int first_number = 10;

                // This will generate a DivideByZeroException when first_number reaches 0
                while (true)
                {
                    var div_result = first_number+1 / first_number;

                    _logger.LogTrace($"Div Result {first_number + 1}/{first_number} = {div_result}");
                    first_number = first_number - 1;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Example");
                throw;
            }
        }
    }
}
