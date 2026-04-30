// tilt-effect.js
document.addEventListener('DOMContentLoaded', () => {
    const attachTiltEffect = () => {
        const tiltElements = document.querySelectorAll('.tilt-element');

        tiltElements.forEach(element => {
            if (element.dataset.tiltAttached) return;
            element.dataset.tiltAttached = 'true';

            element.addEventListener('mousemove', (e) => {
                const rect = element.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                const centerX = rect.width / 2;
                const centerY = rect.height / 2;

                // Amount of tilt - you can tweak this
                const tiltX = ((y - centerY) / centerY) * -10; 
                const tiltY = ((x - centerX) / centerX) * 10; 

                // Apply transform
                element.style.transform = `perspective(1000px) rotateX(${tiltX}deg) rotateY(${tiltY}deg) scale3d(1.02, 1.02, 1.02)`;
                element.style.transition = 'transform 0.1s ease-out';
            });

            element.addEventListener('mouseleave', () => {
                element.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg) scale3d(1, 1, 1)';
                element.style.transition = 'transform 0.5s ease-in-out';
            });
        });
    };

    // Attach initially
    attachTiltEffect();

    // Re-attach if DOM changes (useful for dynamically loaded content)
    const observer = new MutationObserver(attachTiltEffect);
    observer.observe(document.body, { childList: true, subtree: true });
});
