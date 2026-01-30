import { Component, OnInit, inject, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SessionStateService, ConfigStateService } from '@abp/ng.core';

interface Language {
  cultureName: string;
  displayName: string;
  flagIcon: string;
}

@Component({
  selector: 'app-configuracion',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './configuracion.component.html',
  styleUrls: ['./configuracion.component.scss'],
})
export class ConfiguracionComponent implements OnInit {
  languages: Language[] = [];
  currentLanguage: string = '';
  showLanguageDropdown = false;

  private sessionState = inject(SessionStateService);
  private configState = inject(ConfigStateService);
  private router = inject(Router);
  private elementRef = inject(ElementRef);

  ngOnInit() {
    this.loadLanguages();
    this.currentLanguage = this.sessionState.getLanguage() || 'es';
  }

  loadLanguages() {
    const localization = this.configState.getDeep('localization');
    if (localization && localization.languages) {
      this.languages = localization.languages.map((lang: any) => ({
        cultureName: lang.cultureName,
        displayName: lang.displayName,
        flagIcon: this.getFlagIcon(lang.cultureName),
      }));
    } else {
      // Idiomas por defecto si no hay configuración
      this.languages = [
        { cultureName: 'es', displayName: 'Español', flagIcon: '🇪🇸' },
        { cultureName: 'en', displayName: 'English', flagIcon: '🇺🇸' },
      ];
    }
  }

  getFlagIcon(cultureName: string): string {
    const flags: { [key: string]: string } = {
      ar: '🇸🇦',
      'zh-Hans': '🇨🇳',
      'zh-Hant': '🇹🇼',
      cs: '🇨🇿',
      en: '🇺🇸',
      'en-GB': '🇬🇧',
      fi: '🇫🇮',
      fr: '🇫🇷',
      'de-DE': '🇩🇪',
      hi: '🇮🇳',
      hu: '🇭🇺',
      is: '🇮🇸',
      it: '🇮🇹',
      'pt-BR': '🇧🇷',
      'ro-RO': '🇷🇴',
      ru: '🇷🇺',
      sk: '🇸🇰',
      es: '🇪🇸',
      sv: '🇸🇪',
      tr: '🇹🇷',
    };
    return flags[cultureName] || '🌐';
  }

  changeLanguage(cultureName: string) {
    this.sessionState.setLanguage(cultureName);
    this.currentLanguage = cultureName;
    this.showLanguageDropdown = false;
    // Recargar la página para aplicar los cambios de idioma
    window.location.reload();
  }

  isCurrentLanguage(cultureName: string): boolean {
    return this.currentLanguage === cultureName;
  }

  getCurrentLanguageObject(): Language | undefined {
    return this.languages.find(lang => lang.cultureName === this.currentLanguage);
  }

  toggleLanguageDropdown(event: Event) {
    event.stopPropagation();
    this.showLanguageDropdown = !this.showLanguageDropdown;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside && this.showLanguageDropdown) {
      this.showLanguageDropdown = false;
    }
  }

  goBack() {
    this.router.navigate(['/']);
  }
}
