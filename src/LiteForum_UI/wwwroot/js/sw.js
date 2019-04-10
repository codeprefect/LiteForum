self.addEventListener('install', (event) => {
  let CACHE_NAME = 'xyz-cache'
  let urlsToCache = [
      '/',
      '/css/site.css',
      '/css/bootstrap/bootstrap.min.css'
  ];
  // fetch("https://localhost:5001/_framework/_bin/System.Runtime.CompilerServices.Unsafe.dll", {"credentials":"include","headers":{"accept":"*/*","accept-language":"en-GB,en-US;q=0.9,en;q=0.8"},"referrer":"https://localhost:5001/","referrerPolicy":"no-referrer-when-downgrade","body":null,"method":"GET","mode":"cors"});
  event.waitUntil(
      /* open method available on caches, takes in the name of cache as the first parameter. It returns a promise that resolves to the instance of cache
      All the URLS above can be added to cache using the addAll method. */
      caches.open(CACHE_NAME)
      .then (cache => cache.addAll(urlsToCache))
  );
});
