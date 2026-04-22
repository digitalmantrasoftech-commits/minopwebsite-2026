(function($) {
"use strict";
// Hide Loading Box (Preloader)
function handlePreloader() { $(".preloader").length && $(".preloader").delay(200).fadeOut(500) }
// Update Header Style and Scroll to Top
function headerStyle() { var e, a, d, n; $(".main-header").length && (e = $(window).scrollTop(), a = $(".main-header"), d = $(".main-header .sticky-header, .header-style-five"), n = $(".scroll-to-top"), 100 < e ? (a.addClass("fixed-header"), d.addClass("animated slideInDown"), n.fadeIn(300)) : (a.removeClass("fixed-header"), d.removeClass("animated slideInDown"), n.fadeOut(300))) }
headerStyle();
// Submenu Dropdown Toggle
$(".main-header li.dropdown ul").length && ($(".main-header li.dropdown").append('<div class="dropdown-btn"><span class="fa fa-angle-down"></span></div>'), $(".main-header li.dropdown .dropdown-btn").on("click", function () { $(this).prev("ul").slideToggle(500) }), $(".main-header .navigation li.dropdown > a,.hidden-bar .side-menu li.dropdown > a").on("click", function (n) { n.preventDefault() }));
// Banner Carousel
$(".banner-carousel").length && $(".banner-carousel").owlCarousel({ animateOut: "slideOutDown", animateIn: "fadeIn", loop: !0, margin: 0, nav: !0, singleItem: !0, smartSpeed: 500, autoHeight: !1, autoplay: !0, autoplayTimeout: 1e4, navText: ['<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>'], responsive: { 0: { items: 1 }, 600: { items: 1 }, 1024: { items: 1 } } });
// Single Item Carousel
$(".single-item-carousel").length && $(".single-item-carousel").owlCarousel({ loop: !0, margin: 0, nav: !0, smartSpeed: 500, autoplay: !0, navText: ['<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>'], responsive: { 0: { items: 1 }, 600: { items: 1 }, 1200: { items: 1 } } });
// Features Carousel
$(".cases-carousel").length && $(".cases-carousel").owlCarousel({ loop: !0, margin: 0, nav: !1, smartSpeed: 700, autoplay: !0, responsive: { 0: { items: 1 }, 600: { items: 2 }, 1024: { items: 3 }, 1280: { items: 4 } } });
// Projects Carousel
$(".projects-carousel").length && $(".projects-carousel").owlCarousel({ loop: !0, margin: 30, nav: !1, smartSpeed: 700, autoplay: !0, responsive: { 0: { items: 1 }, 600: { items: 1 }, 768: { items: 2 }, 1024: { items: 3 } } });
// Sponsors Carousel
$(".sponsors-carousel").length && $(".sponsors-carousel").owlCarousel({ loop: !0, margin: 30, nav: !0, smartSpeed: 500, autoplay: !0, navText: ['<span class="flaticon-back"></span>', '<span class="flaticon-next-1"></span>'], responsive: { 0: { items: 1 }, 600: { items: 2 }, 768: { items: 3 }, 1024: { items: 4 } } });
// Testimonials Carousel
$(".testimonials-carousel").length && $(".testimonials-carousel").owlCarousel({ loop: !0, margin: 0, nav: !1, smartSpeed: 300, autoplay: !0, navText: ['<span class="flaticon-back"></span>', '<span class="flaticon-next-1"></span>'], responsive: { 0: { items: 1 }, 600: { items: 1 }, 1024: { items: 1 } } });
// Accordion Box
$(".accordion-box").length && $(".accordion-box").on("click", ".acc-btn", function () { var c = $(this).parents(".accordion-box"), i = $(this).parents(".accordion"); if (!0 !== $(this).hasClass("active") && $(c).find(".accordion .acc-btn").removeClass("active"), $(this).next(".acc-content").is(":visible")) return !1; $(this).addClass("active"), $(c).children(".accordion").removeClass("active-block"), $(c).find(".accordion").children(".acc-content").slideUp(300), i.addClass("active-block"), $(this).next(".acc-content").slideDown(300) });
// Tabs Box
$(".tabs-box").length && $(".tabs-box .tab-buttons .tab-btn").on("click", function (t) { t.preventDefault(); t = $($(this).attr("data-tab")); if ($(t).is(":visible")) return !1; t.parents(".tabs-box").find(".tab-buttons").find(".tab-btn").removeClass("active-btn"), $(this).addClass("active-btn"), t.parents(".tabs-box").find(".tabs-content").find(".tab").fadeOut(0), t.parents(".tabs-box").find(".tabs-content").find(".tab").removeClass("active-tab animated fadeIn"), $(t).fadeIn(0), $(t).addClass("active-tab animated fadeIn") });
// Fact Counter + Text Count
$(".count-box").length && $(".count-box").appear(function () { var t = $(this), n = t.find(".count-text").attr("data-stop"), o = parseInt(t.find(".count-text").attr("data-speed"), 10); t.hasClass("counted") || (t.addClass("counted"), $({ countNum: t.find(".count-text").text() }).animate({ countNum: n }, { duration: o, easing: "linear", step: function () { t.find(".count-text").text(Math.floor(this.countNum)) }, complete: function () { t.find(".count-text").text(this.countNum) } })) }, { accY: 0 });
// Custom Seclect Box
$(".custom-select-box").length && $(".custom-select-box").selectmenu().selectmenu("menuWidget").addClass("overflow");
// LightBox / Fancybox
$(".lightbox-image").length && $(".lightbox-image").fancybox({ openEffect: "fade", closeEffect: "fade", helpers: { media: {} } });
// Sortable Masonary with Filters
function sortableMasonry() { var i, t, a, n; $(".sortable-masonry").length && (i = $(window), t = $(".sortable-masonry .items-container"), a = $(".filter-btns"), t.isotope({ filter: "*", masonry: { columnWidth: 2 }, animationOptions: { duration: 500, easing: "linear" } }), a.find("li").on("click", function () { var i = $(this).attr("data-filter"); try { t.isotope({ filter: i, animationOptions: { duration: 500, easing: "linear", queue: !1 } }) } catch (i) { } return !1 }), i.on("resize", function () { var i = a.find("li.active").attr("data-filter"); t.isotope({ filter: i, animationOptions: { duration: 500, easing: "linear", queue: !1 } }) }), (n = $(".filter-btns li")).on("click", function () { var i = $(this); i.hasClass("active") || (n.removeClass("active"), i.addClass("active")) })) }
sortableMasonry();
// Default Masonary
function defaultMasonry() { var n, i; $(".masonry-items-container").length && (n = $(window), (i = $(".masonry-items-container")).isotope({ itemSelector: ".masonry-item", masonry: { columnWidth: 1 }, animationOptions: { duration: 500, easing: "linear" } }), n.on("resize", function () { i.isotope({ itemSelector: ".masonry-item", animationOptions: { duration: 500, easing: "linear", queue: !1 } }) })) }
defaultMasonry();
// Scroll to a Specific Div
$(".scroll-to-target").length && $(".scroll-to-target").on("click", function () { var t = $(this).attr("data-target"); $("html, body").animate({ scrollTop: $(t).offset().top }, 1500) });
// Elements Animation
var wow; $(".wow").length && (wow = new WOW({ boxClass: "wow", animateClass: "animated", offset: 0, mobile: !1, live: !0 })).init();
// Gallery Filters
$(".filter-list").length && $(".filter-list").mixItUp({});
// scroll
$(window).on("scroll", function () { headerStyle() });
// loader
$(window).on("load", function () { handlePreloader(), defaultMasonry(), sortableMasonry() });
})(window.jQuery);