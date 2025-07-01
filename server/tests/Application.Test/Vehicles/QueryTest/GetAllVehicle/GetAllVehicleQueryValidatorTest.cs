using System;
using Application.Features.Vehicles.Query.GetAllVehicle;

namespace Application.Test.Vehicles.QueryTest.GetAllVehicle;

public class GetAllVehicleQueryValidatorTest
{
    private readonly GetAllVehicleQueryValidator _validator;

    public GetAllVehicleQueryValidatorTest()
    {
        _validator = new GetAllVehicleQueryValidator();
    }    
}
