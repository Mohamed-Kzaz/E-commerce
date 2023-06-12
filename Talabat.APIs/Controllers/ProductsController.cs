using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [CachedAttribute(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithFilterationForCountSpecification(specParams);

            var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

            var paginatedData = new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data);

            return Ok(paginatedData);
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProducts(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));

            var mappedProduct = _mapper.Map<ProductToReturnDto>(product);

            return Ok(mappedProduct);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();

            return Ok(types);
        }


    }
}
