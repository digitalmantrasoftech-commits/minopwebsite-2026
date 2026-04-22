const navbar = document.querySelector('nav');
const links = document.querySelectorAll('.list-li');
const list = document.querySelectorAll('.accordion-item');

list.forEach(res => {
    res.addEventListener('click', e => {
        if (e.target.closest('.accordion-header')) {
            e.target.closest('.accordion-item').classList.toggle('active');
        }
    })
})

function createObserver() {
    var options = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1,
    }

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {

                const curActive = entry.target.id;

                setActiveNavLink(curActive);

                entry.target.classList.add('visible');
            } else {
                entry.target.classList.remove('visible')
            }
        })
    }, options);

    const list = document.querySelectorAll('section');

    list.forEach(i => {
        observer.observe(i)
    })
}

function setActiveNavLink(id) {
    links.forEach(link => link.classList.remove('active'));
    const activeLink = document.getElementById(`nav-${id}`);

    if (activeLink) {
        activeLink.classList.add('active');
    }
}

window.addEventListener('load', createObserver);


// Ap code : not move top any li click in FAQ
document.querySelectorAll('.list-link').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault(); // Stop default anchor scroll behavior

        const targetId = this.getAttribute('href').replace('#', '');

        // Set hash without scroll
        history.replaceState(null, null, `#${targetId}`);

        // Optionally trigger observer manually if needed:
        const section = document.getElementById(targetId);
        if (section) {
            section.scrollIntoView({ block: 'nearest', behavior: 'instant' });
        }
    });
});


// Ap Slider Js code : Begin
$(document).ready(function () {
    initializeSliders()
    let currentSliderId;
    // Fullscreen logic
    function updateFullscreenNav() {
        $('.fullscreen-prev').toggleClass('disabled', currentSlide === 0);
        $('.fullscreen-next').toggleClass('disabled', currentSlide === totalSlides - 1);
    }

    // Open the fullscreen overlay for a specific slide
    function openFullscreen(slideId) {
        
        const $clickedSlide = $('#' + slideId);
        const slideIndex = $clickedSlide.index(); 
        currentSlide = slideIndex;

        const $sliderContainer = $clickedSlide.closest('.slider-container');
        const $slideWrapper = $sliderContainer.find('.slide-wrapper');
        currentSliderId = $sliderContainer.attr('id') || ''; 

        const imgSrc = $clickedSlide.find('img').attr('src');
        $('#fullscreenOverlay img').attr('src', imgSrc);

       
        totalSlides = $slideWrapper.find('.slide-block').length;

        $('#fullscreenOverlay').fadeIn(200);
        updateFullscreenNav();
    }


    // Fullscreen button logic
    $(document).on('click', '.fullscreenBtn', function () {
        
        const slideId = $(this).closest('.slide-block').attr('id');
        openFullscreen(slideId);
    });

    // Fullscreen navigation buttons (prev/next)
  
    $(document).on('click', '.fullscreen-prev', function () {
        
        const $slider = $('#' + currentSliderId).closest('.slider-container');
        const $slideWrapper = $slider.find('.slide-wrapper');

        if (currentSlide > 0) {
            currentSlide--;
            const src = $slideWrapper.find('.slide-block').eq(currentSlide).find('img').attr('src');
            $('#fullscreenOverlay img').attr('src', src);
            updateFullscreenNav();
        }
    });


    $(document).on('click', '.fullscreen-next', function () {
        
        const $slider = $('#' + currentSliderId).closest('.slider-container');
        const $slideWrapper = $slider.find('.slide-wrapper');

        if (currentSlide < totalSlides - 1) {
            currentSlide++;
            const src = $slideWrapper.find('.slide-block').eq(currentSlide).find('img').attr('src');
            $('#fullscreenOverlay img').attr('src', src);
            updateFullscreenNav();
        }
    });


    // Close fullscreen overlay
    $('#closeOverlay').click(function () {
        $('#fullscreenOverlay').fadeOut(200);
    });

    // Normal slider navigation
    function initializeSliders() {
        $('.slider-container').each(function () {
            const slider = $(this);
            let currentSliderIndex = 0;
            const totalSliderSlides = slider.find('.slide-block').length;

            function updateSlider() {
                slider.find('.slide-wrapper').css('transform', `translateX(-${currentSliderIndex * 100}%)`);
                slider.find('.prev').prop('disabled', currentSliderIndex === 0);
                slider.find('.next').prop('disabled', currentSliderIndex === totalSliderSlides - 1);
            }

            // Unbind previous handlers to avoid duplicates
            slider.find('.prev').off('click').on('click', () => {
                if (currentSliderIndex > 0) {
                    currentSliderIndex--;
                    updateSlider();
                }
            });

            slider.find('.next').off('click').on('click', () => {
                if (currentSliderIndex < totalSliderSlides - 1) {
                    currentSliderIndex++;
                    updateSlider();
                }
            });

            updateSlider();
        });
    }

  




    // VJ'S logic
    // Logic for searching
    $("#Glabal_Search").on("keyup", function () {
        const value = $(this).val().toLowerCase();
        const $resultsContainer = $(".searchsection_detail");

        $resultsContainer.empty();

        if (value.length < 3) {
            $("#section_body").show();
            return;
        }

        $("#section_body").hide();

        $(".accordion-item").each(function () {
            const $item = $(this);
            const $title = $item.find(".accordion-title");
            const $body = $item.find(".accordion-body");

            // Store original content only once
            if (!$title.data("original")) $title.data("original", $title.html());
            if (!$body.data("original")) $body.data("original", $body.html());

            const titleText = $title.text().toLowerCase();
            const bodyText = $body.text().toLowerCase();

            if (titleText.includes(value) || bodyText.includes(value)) {
                const $clone = $item.clone();

                // Highlight title
                const originalTitleHtml = $title.data("original");
                const highlightedTitle = originalTitleHtml.replace(
                    new RegExp(`(${value})`, "gi"),
                    `<span style="background-color: #295097; color: #ffffff; border-radius: 4px; padding: 0 4px;">$1</span>`
                );
                $clone.find(".accordion-title").html(highlightedTitle);

           
                const originalBodyHtml = $body.data("original");
                const $cloneBody = $clone.find(".accordion-body");
                $cloneBody.html(originalBodyHtml); 

                highlightTextNodes($cloneBody[0], value);

                $resultsContainer.append($clone);
            }
        });

        // Accordion toggle behavior
        $resultsContainer.find(".accordion-header").on("click", function () {
            const $body = $(this).next(".accordion-body");
            $body.slideToggle();
        });
        initializeSliders();
    });

    // NAV ITEM CLICK CLEARS SEARCH
    $(document).on("click", ".list", function () {
        if ($("#Glabal_Search").val().trim() !== "") {
            $("#Glabal_Search").val("");
            $(".searchsection_detail").empty();
            $("#section_body").show();
        }
    });

   
    function highlightTextNodes(node, keyword) {
        if (node.nodeType === 3) { // Text node
            const text = node.nodeValue;
            const matchRegex = new RegExp(`(${keyword})`, "gi");
            if (matchRegex.test(text)) {
                const spanWrapper = document.createElement("span");
                spanWrapper.innerHTML = text.replace(matchRegex, `<span style="background-color: #295097; color: white; border-radius: 4px; padding: 0 4px;">$1</span>`);
                node.replaceWith(...spanWrapper.childNodes);
            }
        } else if (node.nodeType === 1 && node.nodeName !== "SCRIPT" && node.nodeName !== "STYLE") {
            // Don't enter images, script, style
            for (let i = 0; i < node.childNodes.length; i++) {
                highlightTextNodes(node.childNodes[i], keyword);
            }
        }
    }



    window.downloadImage = function (button) {
        const slideBlock = button.closest('.slide-block');
        const img = slideBlock.querySelector('img');
        const imageUrl = img.src;

       
        const imgRequest = new XMLHttpRequest();
        imgRequest.open('GET', imageUrl, true);
        imgRequest.responseType = 'blob'; 
        imgRequest.onload = function () {
            const blob = imgRequest.response;
            const fileName = imageUrl.split('/').pop().split('?')[0];
            saveAs(blob, fileName);  
        };
        imgRequest.onerror = function () {
            alert('Failed to fetch the image.');
        };
        imgRequest.send();
    };


    //Logic for the serached charcter  and navbar clicked topics.
    $(document).on('click', '.list', function () {
       
        if ($("#Glabal_Search").val().trim() !== '') {
            $("#Glabal_Search").val('');
        }

      
        $("#section_body").show(); 
        $(".searchsection_detail").empty(); 
    });


});
// Ap Slider Js code : End