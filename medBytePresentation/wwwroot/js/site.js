// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {

    //$(".Category").on("click", function () {
        
    //    var products = $(this).data('products');
    //    var max = $(this).data('max');
    //    var catid = $(this).data('id');
    //    var childs = $(this).data('childs');
    //    console.log(products);
    //    $(".productHide").css("display", "none");
    //    if (products.length>1) {
    //        var allCategories = products.split("-");
    //        console.log(products)
    //        console.log(allCategories)
    //        for (i = 0; i < allCategories.length; i++) {
              
    //                $(".category-id-" + allCategories[i]).css("display", "block");
               
    //        }
    //    } else {

    //        $(".category-id-" + catid).css("display", "block");
    //    }

    //});

    $("#all-weapons").on("click", function () {

      
        $(".productHide").css("display", "block");

     

    });



    $("li[data-children]").on("click", function () {
        var children = $(this).data('children');
        var id = $(this).data('self');
        $("#cat-title").text($(this).data("name"));
        $(".productHide").css("display", "none");
        if (children.length > 0) {
            var allCategories = children.split("-");
            
            for (i = 0; i < allCategories.length; i++) {
                
                $(".category-id-" + allCategories[i]).css("display", "block");

            }
        } 
      
        $(".category-id-" + id).css("display", "block");
    });
});