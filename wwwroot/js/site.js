/**
 * Cinema Noir Luxe - Interactive Enhancements
 * Premium movie experience with smooth animations
 */

(function() {
    'use strict';

    // ═══════════════════════════════════════════════════════════════
    // NAVBAR SCROLL EFFECT
    // ═══════════════════════════════════════════════════════════════

    const navbar = document.querySelector('.navbar');
    let lastScrollY = window.scrollY;
    let ticking = false;

    function updateNavbar() {
        if (window.scrollY > 80) {
            navbar?.classList.add('scrolled');
        } else {
            navbar?.classList.remove('scrolled');
        }
        ticking = false;
    }

    window.addEventListener('scroll', function() {
        lastScrollY = window.scrollY;
        if (!ticking) {
            window.requestAnimationFrame(updateNavbar);
            ticking = true;
        }
    }, { passive: true });

    // ═══════════════════════════════════════════════════════════════
    // SMOOTH CAROUSEL SCROLLING
    // ═══════════════════════════════════════════════════════════════

    document.querySelectorAll('.movie-carousel').forEach(carousel => {
        let isDown = false;
        let startX;
        let scrollLeft;
        let velX = 0;
        let momentumID;

        carousel.addEventListener('mousedown', (e) => {
            isDown = true;
            carousel.style.cursor = 'grabbing';
            startX = e.pageX - carousel.offsetLeft;
            scrollLeft = carousel.scrollLeft;
            cancelMomentumTracking();
        });

        carousel.addEventListener('mouseleave', () => {
            if (isDown) {
                isDown = false;
                carousel.style.cursor = 'grab';
                beginMomentumTracking();
            }
        });

        carousel.addEventListener('mouseup', () => {
            isDown = false;
            carousel.style.cursor = 'grab';
            beginMomentumTracking();
        });

        carousel.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - carousel.offsetLeft;
            const walk = (x - startX) * 1.5;
            const prevScrollLeft = carousel.scrollLeft;
            carousel.scrollLeft = scrollLeft - walk;
            velX = carousel.scrollLeft - prevScrollLeft;
        });

        // Momentum scrolling
        function beginMomentumTracking() {
            cancelMomentumTracking();
            momentumID = requestAnimationFrame(momentumLoop);
        }

        function cancelMomentumTracking() {
            cancelAnimationFrame(momentumID);
        }

        function momentumLoop() {
            carousel.scrollLeft += velX;
            velX *= 0.95;
            if (Math.abs(velX) > 0.5) {
                momentumID = requestAnimationFrame(momentumLoop);
            }
        }

        // Set initial cursor
        carousel.style.cursor = 'grab';
    });

    // ═══════════════════════════════════════════════════════════════
    // INTERSECTION OBSERVER FOR ANIMATIONS
    // ═══════════════════════════════════════════════════════════════

    const observerOptions = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    };

    const sectionObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');

                // Animate children cards with stagger
                const cards = entry.target.querySelectorAll('.movie-card');
                cards.forEach((card, index) => {
                    card.style.animationDelay = `${index * 0.05}s`;
                });
            }
        });
    }, observerOptions);

    document.querySelectorAll('.movie-section').forEach(section => {
        sectionObserver.observe(section);
    });

    // ═══════════════════════════════════════════════════════════════
    // SMOOTH PAGE TRANSITIONS
    // ═══════════════════════════════════════════════════════════════

    document.addEventListener('DOMContentLoaded', () => {
        document.body.style.opacity = '0';
        requestAnimationFrame(() => {
            document.body.style.transition = 'opacity 0.5s ease';
            document.body.style.opacity = '1';
        });
    });

    // ═══════════════════════════════════════════════════════════════
    // SEARCH INPUT ENHANCEMENT
    // ═══════════════════════════════════════════════════════════════

    const searchInputs = document.querySelectorAll('.search-input, .search-input-large');
    searchInputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement?.classList.add('focused');
        });

        input.addEventListener('blur', function() {
            this.parentElement?.classList.remove('focused');
        });
    });

    // ═══════════════════════════════════════════════════════════════
    // MOVIE CARD TILT EFFECT (Subtle 3D)
    // ═══════════════════════════════════════════════════════════════

    document.querySelectorAll('.movie-card').forEach(card => {
        card.addEventListener('mousemove', function(e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const rotateX = (y - centerY) / 20;
            const rotateY = (centerX - x) / 20;

            this.style.transform = `scale(1.08) translateY(-8px) perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
        });

        card.addEventListener('mouseleave', function() {
            this.style.transform = '';
        });
    });

    // ═══════════════════════════════════════════════════════════════
    // KEYBOARD NAVIGATION
    // ═══════════════════════════════════════════════════════════════

    document.addEventListener('keydown', (e) => {
        // Focus search on '/' key
        if (e.key === '/' && !['INPUT', 'TEXTAREA'].includes(document.activeElement?.tagName)) {
            e.preventDefault();
            document.querySelector('.search-input')?.focus();
        }

        // Escape to blur
        if (e.key === 'Escape') {
            document.activeElement?.blur();
        }
    });

    // ═══════════════════════════════════════════════════════════════
    // PRELOAD IMAGES ON HOVER
    // ═══════════════════════════════════════════════════════════════

    document.querySelectorAll('.movie-card a').forEach(link => {
        link.addEventListener('mouseenter', function() {
            const href = this.getAttribute('href');
            if (href) {
                const preloadLink = document.createElement('link');
                preloadLink.rel = 'prefetch';
                preloadLink.href = href;
                document.head.appendChild(preloadLink);
            }
        }, { once: true });
    });

    // ═══════════════════════════════════════════════════════════════
    // CONSOLE BRANDING
    // ═══════════════════════════════════════════════════════════════

    console.log(
        '%c Lumière %c Your Private Film Collection ',
        'background: linear-gradient(135deg, #d4a853, #e8c97a); color: #0a0a0a; padding: 10px 20px; font-family: Georgia, serif; font-size: 16px; font-weight: bold;',
        'background: #0a0a0a; color: #d4a853; padding: 10px 20px; font-family: system-ui, sans-serif; font-size: 12px;'
    );

})();
