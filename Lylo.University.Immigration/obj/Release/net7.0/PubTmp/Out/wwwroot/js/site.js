// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const links = document.querySelectorAll("nav a");

links.forEach(link => {
    link.addEventListener("mouseenter", () => {
        link.style.transform = "scale(1.1)";
    });
    link.addEventListener("mouseleave", () => {
        link.style.transform = "scale(1)";
    });
});

const observer = new IntersectionObserver((entries) => {
  entries.forEach((entry) => {
    console.log(entry)
    if (entry.isIntersecting) {
      entry.target.classList.add('show');
    } else {
      entry.target.classList.remove('show');
    }
  });
});

const hiddenElements = document.querySelectorAll('.hidden');
hiddenElements.forEach((el) => observer.observe(el));
