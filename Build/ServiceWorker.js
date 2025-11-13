const cacheName = "DefaultCompany-UI_CharacterSelect_Demo01-1.0";
const contentToCache = [
    "Build/c964438cc17e2d327bf0d2aeeeb844a3.loader.js",
    "Build/cce4d86208070d2b9dc5378b525a14e2.framework.js",
    "Build/25c22e10749e2d35365ad224ab2ebdb2.data",
    "Build/da0f69a8108aaef8e8b21e0e8b154483.wasm",
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
