using medByteApi.Controllers;
using medByteApi.Models.ViewModels;
using medByteApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;
using static medByteApi.Controllers.AccountController;

namespace XUnitTest.medByte
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }

        [ExcludeFromCodeCoverage]
        public class CalculatorTests
        {
            //[Fact]
            //public void PassingTest()
            //{


            //    var result = Mock < new IEnumerable<CategoryViewModel>>() ;
            //    var apiService = new Mock<IDbService>();
            //    apiService.Setup(x => x.GetAllCategories().Returns(result.GetType()));
            //    Assert.Equal(result, apiService.Object.GetAllCategories(),);



            //}

            //[Theory]
            //[InlineData("medbytenoreply@gmail.com","Asd123!)]
            //[InlineData(0)]
            //[InlineData(1)]
            //public void IsPrime_ValuesLessThan2_ReturnFalse(int value)
            //{
            //    var result = _primeService.IsPrime(value);

            //    Assert.False(result, $"{value} should not be prime");
            //}

        }
    }
}
