import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SmartSuggestion } from '../models/smart-suggestion.model';
import { AgentSummaryResponse } from '../models/agent-daily-summary.model';
import { BASE_URL } from './common-exports';

@Injectable({
  providedIn: 'root'
})
export class SmartSuggestionsService {

  constructor(private http: HttpClient) { }

  getSmartSuggestions(): Observable<SmartSuggestion[]> {
    return this.http.get<SmartSuggestion[]>(`${BASE_URL}/SmartSuggestions/suggestions`);
  }

  getAgentInsights(): Observable<{ message: string }> {
    return this.http.get<{ message: string }>(`${BASE_URL}/SmartSuggestions/agentinsights`);
  }
  
  getDailySummary(): Observable<AgentSummaryResponse> {
    return this.http.get<AgentSummaryResponse>(`${BASE_URL}/AgentChat/daily-summary`);
  }
}

