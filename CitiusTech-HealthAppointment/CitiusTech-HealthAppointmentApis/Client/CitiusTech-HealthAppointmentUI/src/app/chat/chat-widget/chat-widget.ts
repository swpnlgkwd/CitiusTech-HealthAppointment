import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ElementRef, NgZone, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SmartSuggestion } from '../../core/models/smart-suggestion.model';
import { AgentService } from '../../core/services/agent';
import { SmartSuggestionsService } from '../../core/services/smart-suggestion';
import { QuickReply } from '../../core/models/agent-daily-summary.model';

interface ChatMessage {
  text: string;
  sender: 'user' | 'bot';
  time: string;
  quickReplies?: QuickReply[];
}

@Component({
  selector: 'app-chat-widget',
  templateUrl: './chat-widget.html',
  styleUrls: ['./chat-widget.css'],
  imports: [CommonModule, ReactiveFormsModule],
  standalone: true
})

export class ChatWidgetComponent implements OnInit {
  @ViewChild('chatContainer') chatContainer!: ElementRef;
  isOpen = false;
  chatForm: FormGroup;
  isBotTyping = false;
  messages: ChatMessage[] = [];

  constructor(private fb: FormBuilder, private agentService: AgentService, private smartSuggestionService: SmartSuggestionsService,
    private ngZone: NgZone, private cdRef: ChangeDetectorRef) {
    this.chatForm = this.fb.group({
      message: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadSmartSuggestions();
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
  }

  refreshChat(): void {
    this.messages = [];
    this.agentService.refresh().subscribe({
      next: (response) => {
        console.log(response);
        console.log(response.threadId);
        localStorage.setItem("threadId", response.threadId)
      },
      error: (error) => {
        console.error('Agent error:', error);
      }
    });
  }

  private getCurrentTime(): string {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  scrollToBottom(): void {
    setTimeout(() => {
      try {
        this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
      } catch (e) {
        console.error('Scroll error:', e);
      }
    }, 100);
  }

  sendMessage(action?: string): void {
    if (this.chatForm.invalid) return;

    const userMessage = this.chatForm.value.message;
    const currentTime = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    // Push user message
    this.messages.push({ sender: 'user', text: userMessage, time: currentTime });
    this.scrollToBottom();

    this.chatForm.reset();

    // Show typing indicator
    this.isBotTyping = true;

    // Simulate bot response with delay
    setTimeout(() => {
      this.isBotTyping = false;
      this.messages.push({
        sender: 'bot',
        text: 'This is a simulated bot reply.',
        time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
        quickReplies: [{label:'Book Appointment', value:''}, {label:'View History', value:'Cancel'}]
      });
      this.scrollToBottom();
    }, 2000); // delay for effect
  }

  handleActionClick(s: SmartSuggestion): void {
    const actionMessage = {
      type: s.type,
      actionName: s.actionName,
      data: s.actionData || {}
    };

    this.sendMessage(s.actionText); // This should route to your chat API
  }

  handleQuickReply(reply: string) {
    const currentTime = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    // Push user reply
    this.messages.push({
      sender: 'user',
      text: reply,
      time: currentTime
    });

    // Simulate bot typing
    this.isBotTyping = true;

    setTimeout(() => {
      this.isBotTyping = false;
      this.messages.push({
        sender: 'bot',
        text: `You selected "${reply}". Let me help you further.`,
        time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
      });
    }, 1500);
  }


  loadSmartSuggestions(): void {

    // Show typing animation first
    this.isBotTyping = true;

    // On init or scheduler landing
    // Get daily summary with a delay to simulate typing
    this.smartSuggestionService.getDailySummary().subscribe({
      next: (response) => {
        const message = response?.summaryMessage?.trim();

        // Simulate typing delay (2 seconds)
        setTimeout(() => {
          if (message) {
            this.messages.push({
              sender: 'bot',
              text: message,
              time: this.getCurrentTime(),
              quickReplies: response.quickReplies
            });
          } else {
            this.messages.push({
              sender: 'bot',
              time: this.getCurrentTime(),
              text: `Welcome to MediMate! How can I assist you today?`
            });
          }
          this.isBotTyping = false;
          this.cdRef.detectChanges();
        }, 4000); // 2-second delay
      },
      error: (err: any) => {
        console.error('Failed to load daily agent summary', err);

        // Show fallback message after delay
        setTimeout(() => {
          this.messages.push({
            sender: 'bot',
            time: this.getCurrentTime(),
            text: `Welcome to MediMate! How can I assist you today?`
          });
          this.isBotTyping = false;
          this.cdRef.detectChanges();
        }, 2000); // Delay fallback too
      }
    });
  }

}
