$(document).ready(function(){
    $('.image-slider').slick({
        infinite: true, 
        slidesToShow: 5,
        slidesToScroll: 2,
        prevArrow:"<button type='button' class='slick-prev pull-left'><i class='fa fa-angle-left' aria-hidden='true'><</i></button>",
        nextArrow:"<button type='button' class='slick-next pull-right'><i class='fa fa-angle-right' aria-hidden='true'>></i></button>"
      });
  });
  
  $(document).ready(function(){
    $('.container-banner').slick({
      autoplay: true,
      autoplay:true,
      arrows:false,
      dots:true,
    });
  });