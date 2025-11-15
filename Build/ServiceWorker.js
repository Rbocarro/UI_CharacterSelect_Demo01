const cacheName = "DefaultCompany-UI_CharacterSelect_Demo01-1.0";
const contentToCache = [
    "Build/bbfaa763a2ac80fe8a06db54096bbdcf.loader.js",
    "Build/f589a9d17337c7c597857e05db0c2d5d.framework.js",
    "Build/cfd58d3600034cd94a8e94b2d167f3d9.data",
    "Build/63bd7510d8ba5763052ef65ce6c83f70.wasm",
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
