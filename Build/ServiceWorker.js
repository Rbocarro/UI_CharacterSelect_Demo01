const cacheName = "DefaultCompany-UI_CharacterSelect_Demo01-1.0";
const contentToCache = [
    "Build/038cf38c57b3ae76e16b45869c4f08a6.loader.js",
    "Build/f589a9d17337c7c597857e05db0c2d5d.framework.js",
    "Build/7b29f50fe9965ba7b957abd5621bbbe5.data",
    "Build/d1de6443b3feab200d397a45d7e8320a.wasm",
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
