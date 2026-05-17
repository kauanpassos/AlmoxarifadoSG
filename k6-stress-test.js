import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuração do Estresse:
// Simula 50 usuários virtuais conectando simultaneamente durante 10 segundos.
// O objetivo no pipeline CI/CD é rodar um "Smoke/Stress Test" rápido para garantir que
// as novas alterações de código não destruíram a performance básica ou causaram vazamento de memória.
export const options = {
  vus: 50,
  duration: '10s',
  thresholds: {
    // 95% das requisições DEVEM ser mais rápidas que 500ms
    http_req_duration: ['p(95)<500'],
    // A taxa de erro não pode ultrapassar 1%
    http_req_failed: ['rate<0.01'],
  },
};

export default function () {
  // Bate na API (Endpoint de Health Check) que estará rodando em background no servidor
  const res = http.get('http://localhost:5000/health');
  
  // Validações básicas de resposta
  check(res, {
    'status is 200': (r) => r.status === 200,
    'has correlation id': (r) => r.headers['X-Correlation-Id'] !== undefined,
  });
  
  sleep(1);
}
