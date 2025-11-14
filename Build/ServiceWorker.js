const cacheName = "DefaultCompany-UI_CharacterSelect_Demo01-1.0";
const contentToCache = [
    "Build/1af9b6bca3330a2f8ee594910b1653c5.loader.js",
    "Build/f589a9d17337c7c597857e05db0c2d5d.framework.js",
    "Build/6bd70130dbd6b1ba18262c7d1fa24719.data",
    "Build/51cbd240c95e56af64a3fc3da672b16c.wasm",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
