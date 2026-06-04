import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 50,
  duration: '10s',
  thresholds: {
    http_req_duration: ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
  },
};

export default function () {
  const res = http.get('http://localhost:5000/api/produtos');
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'has correlation id': (r) => r.headers['X-Correlation-Id'] !== undefined,
  });
  
  sleep(1);
}
